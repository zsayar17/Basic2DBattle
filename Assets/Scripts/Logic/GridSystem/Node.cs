/*
    Amac:
        -A * algoritmasinda kullanilmaktadir. Her hucre icin ozeldir.
*/

public struct Node {
    #region Variables

    public unsafe Cell  *ParentCell; // hangi objeden gelindigini tutan isaretci
    public int        GCost;
    private int       _HCost;
    public int FCost { get => GCost + _HCost; }
    #endregion

    #region Public Attributes
    /*
        node'un degerlerinin set edildigi metoddur.
    */
    public unsafe void SetNode(int gCost, int hCost, Cell* parentCell) {
        GCost = gCost;
        _HCost = hCost;
        ParentCell = parentCell;
    }

    /*
        operator overloading kullanilarak iki node kiyaslanir f costu daha dusuk olmadigi kontrol edilir eger esit iseler h costa gore kiyaslanma tekrar yapilir.
    */
    public static bool operator <(Node node1, Node node2) {

        if (node1.FCost < node2.FCost || (node1.FCost == node2.FCost && node1._HCost < node2._HCost)) return true;
        return false;
    }

    public static bool operator >(Node node1, Node node2) => !(node1 < node2);
    #endregion
}
