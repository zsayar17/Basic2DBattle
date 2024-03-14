using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
    Grid sisteminin yonetildigi classtir.
    hucreler bir arrayde tutulmus ve isaretciler uzerinden islemler yapilmistir.
    bunın sebebi referans tipli olmayan hucreleri disarida da kullanabilmek.
*/

public class GridCoordinator {

    private static GridCoordinator _instance;
    public static GridCoordinator Instance {
        get => _instance == null ? (_instance = new GridCoordinator()) : _instance;
    }
    private Grid _Grid; // Gridin referansi
    private  Tilemap[] _Tilemaps; // Gridin sahip oldugu tilemapler
    private Cell[,]    _Cells; // Hucrelerin tutuldugu dizi
    public Vector2Int _GridSize; // Grid boyutu
    private Vector2Int _BeginCords; // Gridin baslangic kordinati

    private GridCoordinator() {

        SetGridAttributes();
        CreateCells();
    }

    /*
        Grid objesinin referansi tilemaplerin refaranslari ve mapin uzunlugunun ayarlandigi metod
    */
    private void SetGridAttributes() {
        HashSet<int> sizex, sizey;

        _Grid = GameObject.FindObjectOfType<Grid>();;
        _Tilemaps = _Grid.GetComponentsInChildren<Tilemap>();

        sizex = new HashSet<int>();
        sizey = new HashSet<int>();
        foreach (Tilemap tilemap in _Tilemaps) {
            sizex.Add(tilemap.cellBounds.max.x);
            sizex.Add(tilemap.cellBounds.min.x);
            sizey.Add(tilemap.cellBounds.max.y);
            sizey.Add(tilemap.cellBounds.min.y);
        }

        _GridSize = new Vector2Int(sizex.Max() - sizex.Min(), sizey.Max() - sizey.Min());
        _BeginCords = new Vector2Int(sizex.Min(), sizey.Min());
    }

    /*
        Grid icerisinde kalan her hucre icin alan tahsis edildigi ve baslatildigi metod
    */
    private unsafe void CreateCells() {
        Vector2Int      cellIndex;
        Vector3Int      cellCord;
        CellSituation   situation;

        _Cells = new Cell[_GridSize.x, _GridSize.y];

        cellIndex = new Vector2Int();
        cellCord = new Vector3Int();
        for (int x = 0; x < _GridSize.x; x++) {
            for (int y = 0; y < _GridSize.y; y++) {
                cellIndex.x = x;
                cellIndex.y = y;
                cellCord.x = x + _BeginCords.x;
                cellCord.y = y + _BeginCords.y;
                situation = GetCellSituation(cellCord);

                _Cells[x, y].SetCell(cellIndex, cellCord, situation, GetCellAdressByIndex(x, y));
            }
        }
    }

    /*
        hucrenin hangi durumda oldugunu kontrol eden metod eger hucre ground mapinde degil ise dolu olarak deger dondurulur.
    */
    private CellSituation GetCellSituation(Vector3Int cellPositon) {
        foreach (Tilemap tilemap in _Tilemaps) {
            if (tilemap.HasTile(cellPositon)) {
                if (tilemap.name != "Ground") return CellSituation.STATIC_FULL;
                return CellSituation.EMPTY;
            }
        }
        return CellSituation.UNKNOWN;
    }

    // Index bilgilerine gore hucrenin isaretcisini alan metot
    private unsafe Cell* GetCellAdressByIndex(int rowIndex, int colIndex) {
        if (rowIndex >= _GridSize.x || rowIndex < 0 ||
            colIndex >= _GridSize.y || colIndex < 0 ) return null;

        fixed (Cell *pNodes = &_Cells[rowIndex, colIndex]) return pNodes;
    }

    // Dunya noktasini kordinat duzleminden hucreye ceviren metot
    public unsafe Cell*  WorldPointToCell (Vector3 worldPosition) {
        Vector3Int pos = _Grid.WorldToCell(worldPosition);

        return GetCellAdressByIndex(pos.x - _BeginCords.x, pos.y - _BeginCords.y);
    }

    /*
        Verilen targetin komsu hucrelerini alip listeye atarak return eden metot
    */
    public unsafe List<IntPtr> GetNeigbours(IBase target) {
        List<IntPtr> neighbours;
        Cell         *leftTopCell;
        int         left, right, top, bottom;

        neighbours = new List<IntPtr>();
        leftTopCell = target.BeginCell;
        left = leftTopCell->cellIndex.x;
        right = left + target.Size.x - 1;
        top = leftTopCell->cellIndex.y;
        bottom = top - target.Size.y + 1;

        for (int x = left - 1; x <= right + 1; x++) {
            for (int y = top + 1; y >= bottom - 1; y--) {
                if (x >= left && x <= right && y <= top && y >= bottom) continue;
                neighbours.Add ((IntPtr)GetCellAdressByIndex(x, y));
            }
        }
        return neighbours;
    }


    // parametre ile gonderilen hucreyi paramete ile gonderilen renge boyayan metot
    public unsafe void ColorizeCell(Cell *cell, Color color) {
        foreach (Tilemap tilemap in _Tilemaps)
        {
            if (tilemap.HasTile(cell->cellPosition)) {
                tilemap.SetTileFlags(cell->cellPosition, TileFlags.None);
                tilemap.SetColor(cell->cellPosition, color);
            }
        }
    }

    // haritayi boyayip haritada ki situation durumlarini anlamayi saglayan metot
    public unsafe void ColorizeMap() {
        Cell *cell;

        for (int x = 0; x < _GridSize.x; x++) {
            for (int y = 0; y < _GridSize.y; y++) {
                cell = GetCellAdressByIndex(x, y);
                if (cell->Situation == CellSituation.EMPTY) ColorizeCell(cell, Color.green);
                else if (cell->Situation == CellSituation.PLACED_FULL) ColorizeCell(cell, Color.blue);
                else ColorizeCell(cell, Color.red);
            }
        }
    }

    /*
        verilen yerlestirilebilr objeyi verilen situation ile yerlestiren metot
        objecet begin cell default olarak null ancak parametre olarak verilirse referans olarak verilen parametre alinir
        eger verilmezse yerlestirilebilir objenin sol ust kismi hedef alinir.
    */
    public unsafe void PlaceObjectOnGrid(IBaseObject placebleObject, CellSituation situation, Cell *objectBeginCell = null) {
        Cell         *checkCell;
        Vector2Int   checkCellPosition;

        checkCell = objectBeginCell == null ? placebleObject.BeginCell : objectBeginCell;
        checkCellPosition = checkCell->cellIndex;
        for (int x = 0; x < placebleObject.Size.x; x++) {
            for (int y = 0; y < placebleObject.Size.y; y++) {
                checkCell = GetCellAdressByIndex(checkCellPosition.x + x, checkCellPosition.y - y);
                checkCell->Situation = situation;
            }
        }
    }

    /*
        gonderilen objenin grid uzerine yerlestirilebilir olup olmadigini kontrol eden metot.
        eger objenin gelecegi yerlerdeki situationlar uygun degilse false return edilir.
    */
    public unsafe bool IsObjectPlaceable(IBaseObject baseObject) {
        Cell         *checkCell;
        Vector2Int   checkCellPosition;

        checkCell = WorldPointToCell(baseObject.BeginPosition);
        if (checkCell == null) return false;
        checkCellPosition = checkCell->cellIndex;
        for (int x = 0; x < baseObject.Size.x; x++) {
            for (int y = 0; y < baseObject.Size.y; y++) {
                checkCell = GetCellAdressByIndex(checkCellPosition.x + x, checkCellPosition.y + y);
                if (checkCell == null || checkCell->Situation != CellSituation.EMPTY) return false;
            }
        }
        return true;
    }
}
