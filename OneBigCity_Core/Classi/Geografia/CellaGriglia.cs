using OneBigCity_Model.Classi.Geografia.EnumGeografia;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneBigCity_Model.Classi.Geografia;

public class CellaGriglia
{
    public Punto3D Posizione { get; }
    public TipoCella Tipo { get; set; }

    public bool IsCalpestabile { get; set; }

    public CellaGriglia(Punto3D posizione, TipoCella tipo, bool isCalpestabile = true)
    {
        Posizione = posizione;
        Tipo = tipo;
        IsCalpestabile = isCalpestabile;
    }

}
