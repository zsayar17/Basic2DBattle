
using UnityEngine;

/*
    Amac
        - Grid'de bulunan her hucrenin sahip oldugu turdur, ozellkile struct secilmistir. Dizide sirali olusturup erisim suresini azaltmak amaclanmistir.
*/
public struct Cell : IBase{
    public Vector3Int       cellPosition; // hucrenin dunya pozisyonu
    public Vector2Int       cellIndex; // hucrenin grid uzerinde ki index verileri
    public CellSituation    Situation { get; set; } // hucrenin durumunu tutan properrt
    public bool IsValid { get => true; }

    public Node             NodeInfo; // yol bulma algoritmasinda kullanilan bilgilerin tutuldugu metot
    private unsafe Cell*    _SelfAdress; // kendisinin adresi

    /*
        - Hucre bilgileri verilen degerlere gore doldururlur.
    */
    public unsafe void SetCell(Vector2Int cellIndex, Vector3Int CellPos, CellSituation nodeSituation, Cell *adress) {
        this.cellIndex.x = cellIndex.x;
        this.cellIndex.y = cellIndex.y;
        this.cellPosition.x = CellPos.x;
        this.cellPosition.y = CellPos.y;
        this._SelfAdress = adress;

        Situation = nodeSituation;
    }

    public unsafe Cell* BeginCell { get => _SelfAdress; readonly set {} }
    public BaseType BaseType { get => BaseType.Cell; }
    public Vector2Int Size { get => Vector2Int.one; }
}
