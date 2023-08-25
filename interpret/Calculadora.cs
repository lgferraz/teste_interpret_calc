using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace interpret
{
    internal class Calculadora
    {
        static int countIden(string identificador, List<object> tolkens)
        {
            int c = 0;
            foreach(object tk in tolkens)
            {
                if (tk is List<tolken>)
                {
                    foreach (object t in (List<tolken>)tk)
                    {
                        tolken t1 = (tolken)t;
                        if (t1.identificador == identificador)
                        {
                            c++;
                        }
                    }
                }
                else
                {
                    tolken t1 = (tolken)tk;
                    if (t1.identificador == identificador)
                    {
                        c++;
                    }
                }
            }
            return c;
        }
        static void Main(string[] args)
        {
            //construindo tolkenizer e parser
            string[] separadores = new string[] { "+", "-", "*", "/" };
            Tolkenizer tolkenizer = new Tolkenizer(separadores);

            string[] tk_soma = new string[] { "+" };
            TipoTolken tipo_soma = new TipoTolken();
            tipo_soma.identificadores = tk_soma;
            tipo_soma.tipo = "soma";
            tipo_soma.conjunto = false;
            string[] tk_subtracao = new string[] { "-" };
            TipoTolken tipo_subtracao = new TipoTolken();
            tipo_subtracao.identificadores = tk_subtracao;
            tipo_subtracao.tipo = "subtracao";
            tipo_subtracao.conjunto = false;
            string[] tk_multiplicacao = new string[] { "*" };
            TipoTolken tipo_multiplicacao = new TipoTolken();
            tipo_multiplicacao.identificadores = tk_multiplicacao;
            tipo_multiplicacao.tipo = "multiplicacao";
            tipo_multiplicacao.conjunto = false;
            string[] tk_divisao = new string[] { "/" };
            TipoTolken tipo_divisao = new TipoTolken();
            tipo_divisao.identificadores = tk_divisao;
            tipo_divisao.tipo = "divisao";
            tipo_divisao.conjunto = false;

            //calculadora
            //tratando entrada
            string entrada = "20*2+20*2+2*4*7+32-82/2";

            tolkenizer.Tokenizar(entrada);
            string[] saida = tolkenizer.Tolkens();

            TipoTolken[] tolkens = new TipoTolken[] { tipo_soma, tipo_subtracao, tipo_multiplicacao, tipo_divisao };
            Parser parser = new Parser(saida, tolkens);

            List<object> tolkens_parser = parser.Parsing();


            //calculando
            int i = 0;
            int qtd_multiplicacao = countIden("*", tolkens_parser);
            int qtd_divisao = countIden("/", tolkens_parser);

            while (i < tolkens_parser.Count)
            {
                tolken tk = (tolken)tolkens_parser[i];
                if (tk.tipo == "multiplicacao")
                {
                    tolken tk1 = (tolken)tolkens_parser[i-1];
                    int tk1n = int.Parse(tk1.identificador);
                    tolken tk2 = (tolken)tolkens_parser[i+1];
                    int tk2n = int.Parse(tk2.identificador);
                    tolken tkr = new tolken();
                    tkr.identificador = (tk1n*tk2n).ToString();
                    tkr.tipo = "null";
                    tolkens_parser.RemoveAt(i-1);
                    tolkens_parser.RemoveAt(i-1);
                    tolkens_parser.RemoveAt(i-1);
                    tolkens_parser.Insert(i-1, tkr);
                    i = 0;
                    qtd_multiplicacao--;
                }
                if (tk.tipo == "divisao" && qtd_multiplicacao == 0)
                {
                    tolken tk1 = (tolken)tolkens_parser[i - 1];
                    int tk1n = int.Parse(tk1.identificador);
                    tolken tk2 = (tolken)tolkens_parser[i + 1];
                    int tk2n = int.Parse(tk2.identificador);
                    tolken tkr = new tolken();
                    tkr.identificador = (tk1n / tk2n).ToString();
                    tkr.tipo = "null";
                    tolkens_parser.RemoveAt(i - 1);
                    tolkens_parser.RemoveAt(i - 1);
                    tolkens_parser.RemoveAt(i - 1);
                    tolkens_parser.Insert(i - 1, tkr);
                    i = 0;
                    qtd_divisao--;
                }
                else if (tk.tipo == "soma" && qtd_multiplicacao == 0 && qtd_divisao == 0)
                {
                    tolken tk1 = (tolken)tolkens_parser[i - 1];
                    int tk1n = int.Parse(tk1.identificador);
                    tolken tk2 = (tolken)tolkens_parser[i + 1];
                    int tk2n = int.Parse(tk2.identificador);
                    tolken tkr = new tolken();
                    tkr.identificador = (tk1n + tk2n).ToString();
                    tkr.tipo = "null";
                    tolkens_parser.RemoveAt(i - 1);
                    tolkens_parser.RemoveAt(i - 1);
                    tolkens_parser.RemoveAt(i - 1);
                    tolkens_parser.Insert(i - 1, tkr);
                    i = 0;
                }
                else if (tk.tipo == "subtracao" && qtd_multiplicacao == 0 && qtd_divisao == 0)
                {
                    tolken tk1 = (tolken)tolkens_parser[i - 1];
                    int tk1n = int.Parse(tk1.identificador);
                    tolken tk2 = (tolken)tolkens_parser[i + 1];
                    int tk2n = int.Parse(tk2.identificador);
                    tolken tkr = new tolken();
                    tkr.identificador = (tk1n - tk2n).ToString();
                    tkr.tipo = "null";
                    tolkens_parser.RemoveAt(i - 1);
                    tolkens_parser.RemoveAt(i - 1);
                    tolkens_parser.RemoveAt(i - 1);
                    tolkens_parser.Insert(i - 1, tkr);
                    i = 0;
                }
                else
                {
                    i++;
                }
            }

            //saida
            tolken r = (tolken)tolkens_parser[0];
            string rs = r.identificador.ToString();
            Console.WriteLine(rs);
            
            Console.ReadKey();
        }
    }
}
