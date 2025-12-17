using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projedeneme.Models;

public class Node
{
    public int Id { get; set; }
    public double Aktiflik { get; set; }
    public int Etkilesim { get; set; }
    public int BaglantiSayisi { get; set; }
    public List<int> KomsuIdler { get; set; } = new();
}
