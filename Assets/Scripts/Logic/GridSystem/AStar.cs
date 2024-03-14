using System;
using System.Collections.Generic;
using System.Linq;

/*
    Amac:
        A star yol bulma algoritmasinin implement edildigi yerdir.
*/

class AStar {

    #region  Static Attributes

    /*
        En kisa yolun hesaplandigi metod, parametre olarak IBase turunden bir target alir eger target tipi hucre
        ise direk hucreyi hedef alarak en kisa yolu bulur eger degil ise obje icin bulan metot
        static olarak disaridan erisilir
    */
    public static unsafe List<IntPtr> FindShortestPath(Cell *startCell, IBase target) {


        if (startCell == null || target == null || target.BeginCell == null) return null;

        if (target.BaseType == BaseType.Cell) return FindPath(startCell, target.BeginCell);

        return FindShortestPathForObject(startCell, target);
    }

    /*
        objeye dogru en kisa yolu bulur.
        once hedefin komsu hucreleri alinir ve bu hucrelere uzakliklar hesaplanir en kisa olandan baslayarak yol
        bulunmaya calisilir eger yol bulunamazsa digerine gecilir
    */
    static unsafe List<IntPtr> FindShortestPathForObject(Cell *startCell, IBase target)
    {
        List<IntPtr> nearestPath;
        List<IntPtr> neigbours;
        List<float> distances;
        int index;

        nearestPath = null;
        neigbours = GridCoordinator.Instance.GetNeigbours(target);
        distances = new List<float>();
        for (int i = 0; i < neigbours.Count; i++)
        {
            if (neigbours[i] != null)
                distances.Add(Utils.GetCellDistance(startCell, (Cell*)neigbours[i]));
            else
                distances.Add(float.MaxValue);
        }

        for (int i = 0; i < distances.Count; i++) {
            index = distances.IndexOf(distances.Min());
            nearestPath = FindPath(startCell, (Cell*)neigbours[index]);
            if (nearestPath != null) return nearestPath;
            distances[index] = float.MaxValue;
        }

        return null;
    }
    #endregion


    #region Private Attributes

    /*
        eger yollanan referanslar bos ise null return edilir ardindan arama islemi yapilir.
    */
    static unsafe List<IntPtr> FindPath(Cell *startCell, Cell *targetCell) {
        List<IntPtr>  openList, closedList;
        Cell*         currentCell;

        if (startCell == targetCell) return new List<IntPtr>();

        if (startCell == null || targetCell == null || targetCell->Situation != CellSituation.EMPTY) return null;

        openList = new List<IntPtr>();
        closedList = new List<IntPtr>();
        openList.Add((IntPtr)startCell);

        while (openList.Count > 0) {
            currentCell = (Cell *)openList[0];
            FindLowestCell(openList, currentCell);
            openList.Remove((IntPtr)currentCell);
            closedList.Add((IntPtr)currentCell);

            if (targetCell == currentCell) return RetracePath(startCell, targetCell);
            SetNeighbours(openList, closedList, currentCell, targetCell);
        }
        return null;
    }
    /*
        en dusuk f cost ya da esitlik durumunda h costa bakan operator overloadin islemi ile nodelar arasi farklara bakan metot
    */
    static unsafe void FindLowestCell(List<IntPtr> openList, Cell *currentCell) {
        Cell *checkCell;

        for (int i = 1; i < openList.Count; i++) {
            checkCell = (Cell*) openList[i];
            if (checkCell->NodeInfo < currentCell->NodeInfo) currentCell = checkCell;
        }
    }

    /*
        Komsu hucrelerin degerlerini hesaplayan eger onceden acik listede bulunuyorsa ve yeni degerler daha iyiyse guncellenen metot
    */
    static unsafe void SetNeighbours(List<IntPtr> openList, List<IntPtr> closedList, Cell *currentCell, Cell *targetCell) {
        Cell *checkCell;
        int distance;

        foreach (IntPtr neighbour in GridCoordinator.Instance.GetNeigbours(*currentCell))
        {
            checkCell = (Cell *)neighbour;
            if (checkCell == null || closedList.Contains(neighbour) || checkCell->Situation != CellSituation.EMPTY) continue;

            distance = Utils.GetCellDistance(checkCell, currentCell) + currentCell->NodeInfo.GCost;

            if (distance < checkCell->NodeInfo.GCost || !openList.Contains(neighbour)) {
                checkCell->NodeInfo.SetNode(distance, Utils.GetCellDistance(checkCell, targetCell), currentCell);

                if (!openList.Contains(neighbour)) openList.Add(neighbour);
            }
        }
    }

    /*
        bulunan yolu ters cevirerek return eden metot
    */

    static unsafe List<IntPtr> RetracePath(Cell *startCell, Cell *targetCell) {
        List<IntPtr> path;
        Cell *currentCell;
        Cell *parentCell;

        path = new List<IntPtr>();
        currentCell = targetCell;
        while (currentCell != startCell) {

            path.Add((IntPtr)currentCell);
            parentCell = currentCell->NodeInfo.ParentCell;
            currentCell = parentCell;
        }
        path.Reverse();
        return path;
    }
    #endregion
}
