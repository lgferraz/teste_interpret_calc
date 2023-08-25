using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interpret
{
    //EstuDados
    public struct TipoTolken
    {
        public string[] identificadores;
        public string tipo;
        public bool conjunto;
    }

    public struct tolken
    {
        public string identificador;
        public string tipo;
    }
    //Tolkenizer
    internal class Tolkenizer
    {
        //percorrer a linha armazenando cada caractere ou tolken
        private int index;
        private string caractere, linha, tolken;
        private List<string> tolkens = new List<string>();
        private string[] separadores;

        public Tolkenizer(string[] separadores)
        {
            this.separadores = separadores;
        }
        public void Tokenizar(string linha)
        {
            this.index = 0;
            this.caractere = "";
            this.tolken = "";
            this.tolkens = new List<string>();
            this.linha = linha;
        }

        private void finalizarTolken()
        {
            if (this.tolken == null)
            {
                this.index++;
            }
            else
            {
                this.tolkens.Add(this.tolken);
                this.tolken = null;
                this.index++;
            }
        }
        private bool identificar()
        {
            if (this.index == linha.Length)
            {
                finalizarTolken();
                return false;
            }

            this.caractere = this.linha[this.index].ToString();

            if (this.caractere == " ")
            {
                finalizarTolken();
                this.caractere = null;
                return true;
            }
            else if (this.separadores.Contains(this.caractere))
            {
                finalizarTolken();
                this.tolkens.Add(this.caractere);
                this.caractere = null;
                return true;
            }
            else
            {
                this.tolken = this.tolken + this.caractere;
                this.index++;
                return true;
            }
        }

        private void acharTolkens()
        {
            bool temTolken = true;
            while (temTolken)
            {
                temTolken = identificar();
            }
        }
        public string[] Tolkens()
        {
            acharTolkens();
            string[] tolkensF = this.tolkens.ToArray();
            return tolkensF;
        }
    }
    //Parser
    internal class Parser
    {
        private string[] entrada;
        private TipoTolken[] tiposTolken;
        private List<object> tolkens = new List<object>();
        private List<string> pilha = new List<string>();
        private int tkConjuntoIndex = 0;
        private tolken tkConjunto;
        private List<tolken> tksConjunto = new List<tolken>();
        private List<List<tolken>> conjuntos = new List<List<tolken>>();
        private tolken tk;
        private bool conjuntoTk;
        private int index;
        public Parser(string[] entrada, TipoTolken[] tiposTolken)
        {
            this.entrada = entrada;
            this.tiposTolken = tiposTolken;
            this.conjuntoTk = false;
            this.index = 0;
        }
        private void conjuntoAdd(string tipo)
        {
            this.tkConjunto.identificador = this.entrada[this.index];
            this.tkConjunto.tipo = tipo;
            this.tksConjunto.Add(this.tkConjunto);
        }
        private void blocoAdd()
        {
            this.conjuntos.Add(this.tksConjunto);
            this.tksConjunto = new List<tolken>();
        }
        private List<tolken> juntar(List<tolken> l1, List<tolken> l2)
        {
            List<tolken> final = new List<tolken>();
            foreach (tolken t in l1)
            {
                final.Add(t);
            }
            foreach (tolken t in l2)
            {
                final.Add(t);
            }
            return final;
        }
        private void finalizaBloco()
        {
            if (this.conjuntos.Count == 1)
            {
                this.tolkens.Add(this.conjuntos[0]);
                this.conjuntos.Clear();

            }
            else if (this.conjuntos.Count > 1)
            {
                this.conjuntos[this.conjuntos.Count - 2] = juntar(conjuntos[this.conjuntos.Count - 2], conjuntos[this.conjuntos.Count - 1]);
                this.conjuntos.RemoveAt(this.conjuntos.Count - 1);
            }
            else
            {
                //
            }

        }
        private bool conjunto()
        {
            if (this.pilha.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private string[] identificarTipo(string tolken)
        {
            string[] tipoR = new string[2];
            tipoR[0] = "null";
            tipoR[1] = "False";
            foreach (TipoTolken tipo in this.tiposTolken)
            {
                if (tipo.identificadores.Contains(tolken))
                {
                    tipoR[0] = tipo.tipo;
                    tipoR[1] = tipo.conjunto.ToString();
                    return tipoR;
                }
            }
            return tipoR;
        }
        private string tolkenProximo(string tolken)
        {
            foreach (TipoTolken tipo in this.tiposTolken)
            {
                if (Array.IndexOf(tipo.identificadores, tolken) + 2 <= tipo.identificadores.Length && tipo.identificadores.Contains(tolken))
                {
                    return tipo.identificadores[Array.IndexOf(tipo.identificadores, tolken) + 1];
                }
            }
            return "null";
        }
        private void finalizar()
        {
            this.tolkens.Add(this.tk);
            this.tk = new tolken();
            this.index++;
        }
        private void finalizarConjunto()
        {
            finalizaBloco();
            finalizaBloco();
            //this.conjuntos = new List<List<tolken>>();
            this.index++;
        }
        private void identificar()
        {
            string[] tipoR = identificarTipo(this.entrada[this.index]);
            this.conjuntoTk = conjunto();
            //identifica tolken
            if (tipoR[1] == "False" && this.conjuntoTk == false)
            {
                this.tk.identificador = this.entrada[this.index];
                this.tk.tipo = tipoR[0];
                finalizar();
            }
            //comeca um bloco
            else if (tipoR[1] == "True" && this.conjuntoTk == false)
            {

                this.tk.identificador = this.entrada[this.index];
                this.tkConjuntoIndex += 1;
                conjuntoAdd(tipoR[0]);
                this.pilha.Add(this.entrada[this.index]);
                this.index++;
            }
            //finaliza o ultimo bloco da pilha
            else if (tipoR[1] == "True" && this.entrada[this.index] == tolkenProximo(this.pilha.Last()) && this.pilha.Count == 1)
            {
                this.tk.identificador += this.entrada[this.index];
                this.pilha.RemoveAt(this.pilha.Count - 1);
                this.tk.tipo = tipoR[0];
                conjuntoAdd(tipoR[0]);
                blocoAdd();
                finalizaBloco();
                finalizarConjunto();

            }
            //finaliza um bloco da pilha
            else if (tipoR[1] == "True" && this.entrada[this.index] == tolkenProximo(this.pilha.Last()) && this.pilha.Count > 1)
            {

                this.tk.identificador += this.entrada[this.index];
                conjuntoAdd(tipoR[0]);
                blocoAdd();
                finalizaBloco();
                this.tkConjuntoIndex -= 1;
                this.pilha.RemoveAt(this.pilha.Count - 1);
                this.index++;

            }
            //comeca um bloco dentro da pilha
            else if (tipoR[1] == "True")
            {
                this.tk.identificador += this.entrada[this.index];
                this.tkConjuntoIndex += 1;
                conjuntoAdd(tipoR[0]);
                blocoAdd();
                this.pilha.Add(this.entrada[this.index]);
                this.index++;
            }
            //adiciona um tolken no bloco
            else
            {
                this.tk.identificador += this.entrada[this.index];
                conjuntoAdd(tipoR[0]);
                this.index++;
            }
        }
        public List<object> Parsing()
        {
            foreach (string tk in entrada)
            {
                identificar();
            }
            return tolkens;
        }
    }
}
