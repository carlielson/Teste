using Newtonsoft.Json;
using ProvaConsole.Model;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using static System.Console;
namespace ProvaConsole
{
    class Program
    {
        static HttpClient _client = new HttpClient();
        static void Main(string[] args)
        {
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.BaseAddress = new Uri("https://localhost:44386");
            Timer t = new Timer(TimerCallback, null, 0, 120000);
            ReadKey();
        }

        private static void TimerCallback(Object o)
        {
            WriteLine($"Execução: {DateTime.Now}" );
            WriteLine($"Inicio - GetItemFila - {DateTime.Now} ");
            GetItemFila();
            GC.Collect();
        }

        static async void GetItemFila()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {

                var itens = new List<Item>();
                WriteLine("Acessar - Endpoint Fila");
                HttpResponseMessage response = await _client.GetAsync("/api/fila");

                if (response.IsSuccessStatusCode)
                {
                    var itemJsonString = await response.Content.ReadAsStringAsync();
                    itens = JsonConvert.DeserializeObject<List<Item>>(itemJsonString);

                    var dadosMoeda = new List<DadosMoeda>();

                    GetMoedaCotacao(dadosMoeda);

                    var listaMoedaCotacaoSel = new List<DadosMoeda>();

                    foreach (var item in itens)
                    {
                        listaMoedaCotacaoSel.AddRange(dadosMoeda.Where(x => x.IdMoeda == item.Moeda && (x.DataReferencia >= item.DataInicio && x.DataReferencia <= item.DataFim)).ToList());
                    }

                    foreach (var item in listaMoedaCotacaoSel)
                    {
                        var cotacao = MoedaCotacao._ListMoedaCotacao.FirstOrDefault(x => x.IdMoeda == item.IdMoeda);
                        if (cotacao != null)
                            item.CodigoCotacao = cotacao.CodCotacao;
                    }

                    List<DadosCotacao> dadosCotacao = GetCotacao();

                    foreach (var item in listaMoedaCotacaoSel)
                    {
                        var dados = dadosCotacao.Where(x => x.CodigoCotacao == item.CodigoCotacao && x.DataCotacao == item.DataReferencia).ToList();
                        if (dados != null)
                            item.ValorCotacao = dados.FirstOrDefault().ValorCotacao;
                    }

                    CreateFileCsv(listaMoedaCotacaoSel);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    WriteLine("Não existe objeto a ser retornado");
            }
            catch (HttpRequestException ex)
            {
                WriteLine($"Ocorreu um erro ao acessar a api: {DateTime.Now} -  Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                WriteLine($"Ocorreu um erro: {DateTime.Now} -  Error: {ex.Message}");
            }
            finally
            {
                stopwatch.Stop();
                WriteLine($"Tempo total de Processamento: {stopwatch.Elapsed}");
                Finalizacao();
            }
           
        }

        private static void Finalizacao()
        {
            WriteLine($"Fim - GetItemFila {DateTime.Now}");
            WriteLine($"------------------------------------------------------------");
            WriteLine();
            WriteLine();
        }

        private static void CreateFileCsv(List<DadosMoeda> listaMoedaCotacaoSel)
        {
            Console.WriteLine("Criar - Arquivo .csv Resultado");

            var csv = new StringBuilder();
            var linhaZero = string.Format("moeda;data;vl_cotacao");
            csv.AppendLine(linhaZero);

            foreach (var item in listaMoedaCotacaoSel)
            {
                var primeiro = item.IdMoeda;
                var segundo = item.DataReferencia.ToString("dd/MM/yyyy");
                var terceiro = item.ValorCotacao.ToString();
                var novaLinha = string.Format("{0};{1};{2}", primeiro, segundo, terceiro);
                csv.AppendLine(novaLinha);
            }

            var fileName = $@"Resultado_{DateTime.Now.ToString("yyyyMMdd")}_{DateTime.Now.ToString("HHmmss")}.csv";
            File.WriteAllText(fileName, csv.ToString());
            Console.WriteLine($"Aquivo gerado, nome: {fileName}");
        }

        private static List<DadosCotacao> GetCotacao()
        {
            WriteLine("Ler - Arquivo DadosCotacao.csv");
            var dadosCotacao = new List<DadosCotacao>();
            using (var reader = new StreamReader(@"DadosCotacao.csv"))
            {
                int contador = 0;
                while (!reader.EndOfStream)
                {
                    contador++;
                    var linha = reader.ReadLine();
                    var valor = linha.Split(';');
                    if (contador == 1)
                    {
                        continue;
                    }

                    dadosCotacao.Add(new DadosCotacao { ValorCotacao = decimal.Parse(valor[0]), CodigoCotacao = int.Parse(valor[1]), DataCotacao = DateTime.Parse(valor[2]) });
                }
            }

            return dadosCotacao;
        }

        private static void GetMoedaCotacao(List<DadosMoeda> dadosMoeda)
        {
            WriteLine("Ler - Arquivo DadosMoeda.csv");

            using (var reader = new StreamReader(@"DadosMoeda.csv"))
            {
                int contador = 0;
                while (!reader.EndOfStream)
                {
                    contador++;
                    var linha = reader.ReadLine();
                    var values = linha.Split(';');
                    if (contador == 1)
                    {
                        continue;
                    }

                    dadosMoeda.Add(new DadosMoeda { IdMoeda = values[0], DataReferencia = DateTime.Parse(values[1]) });
                }
            }
        }
    }
}
