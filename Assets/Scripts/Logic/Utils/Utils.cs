using System;
using UnityEngine;


/*
    Amac:
        Logic kisminda kullanilan genel araclari iceren class
*/

public class Utils
{
    /*
        Pozisyona donderme metotlarinin listesi
    */
    public static Action<IBaseObject>[] LookMethods = SetLookMethods();

    /*
        Verilen size ve hucreye gore hizalanmis pozisyon (resimlrein merkez noktalari genelde ortalari oluyor o yuzden hucreye hizalanmasi
        icin kullanilan metot)
    */

    public static unsafe Vector3 AllignedPositionByCell(Cell *cell, Vector3 size)
        => new Vector3(cell->cellPosition.x + size.x / 2, cell->cellPosition.y + 1 - size.y / 2, 0);


    /*
        İki cell arasinda ki farka gore objenin nereye bakacagini hesaplayan metot
    */
    public unsafe static Direction GetDirectionByCompareCell(Cell *prevCell, Cell *currentCell) {
        Vector2Int delta;

        delta = currentCell->cellIndex - prevCell->cellIndex;

        if (delta.x == -1 && delta.y == -1) return Direction.TOP_LEFT;
        else if (delta.x == -1 && delta.y == 0) return Direction.LEFT;
        else if (delta.x == -1 && delta.y == 1) return Direction.BOTTOM_LEFT;
        else if (delta.x == 0 && delta.y == -1) return Direction.TOP;
        else if (delta.x == 0 && delta.y == 1) return Direction.BOTTOM;
        else if (delta.x == 1 && delta.y == -1) return Direction.TOP_RIGT;
        else if (delta.x == 1 && delta.y == 0) return Direction.RIGHT;
        else if (delta.x == 1 && delta.y == 1) return Direction.BOTTOM_RIGHT;

        return Direction.UNKNOWN;
    }

        /*
            iki cell arasinda ki Manatthan uzakligini olcen metot
        */
        public unsafe static int GetCellDistance(Cell *src, Cell* dest) {
        int distX, distY;

        distX = Math.Abs(src->cellIndex.x - dest->cellIndex.x);
        distY = Math.Abs(src->cellIndex.y - dest->cellIndex.y);

        return distX > distY ? (14 * distY + 10 * (distX - distY)) : (14 * distX + 10 * (distY - distX));
    }

    // mouse pozisyonunu dunya pozisyonuna donusturen metot
    static public Vector3 ScreenToWorld { get => Camera.main.ScreenToWorldPoint(Input.mousePosition); }

    //mouse pozisyonunu hucreye ceviren metot
    static unsafe public Cell* ScreenToCell { get => GridCoordinator.Instance.WorldPointToCell(ScreenToWorld); }

    // mouse pozisyonunu objeye ceviren metot
    static public BaseObject ScreenToObject {
        get {
            Vector3    clickPosition;

            clickPosition = ScreenToWorld;
            clickPosition.z = 0;
            return PositionToObject(clickPosition);
        }
    }

    // mouse pozisyonunu Base ceviren metot (hucre ya da obje)
    static unsafe public IBase ScreenToBase {
        get {
            IBaseObject baseObject;
            Cell *cell;

            baseObject = ScreenToObject;
            if (baseObject == null) {
                cell = ScreenToCell;
                if (cell == null) return null;
                return *cell;
            }
            return baseObject;
        }
    }

    // verilen pozisyon uzerinden objeye carpip carpmadigini olcup geriye carpan objeyi gonderen metot
    static public BaseObject PositionToObject(Vector3 position) {
        BaseObject baseObject;
        RaycastHit2D hit;

        hit = Physics2D.Raycast(position, Vector2.zero);
        if (hit.collider != null) {
            baseObject = hit.collider.gameObject.GetComponent<BaseObject>();
            if (baseObject != null) return baseObject;
        }
        return null;
    }

    // verilen objenin verilen hedef noktasina speed hizinda hareket ettirilmesi
    public static Vector3 MoveToWards(IMoveable moveableObject, Vector3 targetPosition, float speed) {
        Vector3 newPosition;
        Vector3 objectPosition;

        objectPosition = moveableObject.BeginPosition;
        newPosition = Vector3.MoveTowards(objectPosition, targetPosition, speed * Time.deltaTime);
        newPosition.x += moveableObject.SpriteSize.x / 2;
        newPosition.y += moveableObject.SpriteSize.y / 2;
        return newPosition;
    }

    // verilen objenin grid uzerine yerlestirilebilirligini GridCoordinator uzerinden kontrol eden metot
    public static bool IsPlaceable(IBaseObject placebleObject) => GridCoordinator.Instance.IsObjectPlaceable(placebleObject);


    // look methodlarinin initilizar edildigi yer
    static Action<IBaseObject>[] SetLookMethods() {
        Action<IBaseObject>[] actionMehods;

        actionMehods = new Action<IBaseObject>[(int)Direction.size - 1];

        actionMehods[(int)Direction.TOP_LEFT] = LookAtTopLeft;
        actionMehods[(int)Direction.TOP] = LookAtTop;
        actionMehods[(int)Direction.TOP_RIGT] = LookAtTopRight;
        actionMehods[(int)Direction.BOTTOM_LEFT] = LookAtBottomLeft;
        actionMehods[(int)Direction.BOTTOM_RIGHT] = LookAtBottomRight;
        actionMehods[(int)Direction.BOTTOM] = LookAtBottom;
        actionMehods[(int)Direction.LEFT] = LookAtLeft;
        actionMehods[(int)Direction.RIGHT] = LookAtRight;
        return (actionMehods);
    }

    static public void LookAt(IBaseObject baseObject, Direction direction) => LookMethods[(int)direction](baseObject);
    static void LookAtTopLeft(IBaseObject baseObject) => baseObject.ChangeRotation(Quaternion.Euler(0, 0, 45), true);
    static void LookAtLeft(IBaseObject baseObject) => baseObject.ChangeRotation(Quaternion.Euler(0, 0, 0), true);
    static void LookAtBottomLeft(IBaseObject baseObject) => baseObject.ChangeRotation(Quaternion.Euler(0, 0, -45), true);
    static void LookAtTopRight(IBaseObject baseObject) => baseObject.ChangeRotation(Quaternion.Euler(0, 0, -45), false);
    static void LookAtRight(IBaseObject baseObject) => baseObject.ChangeRotation(Quaternion.Euler(0, 0, 0), false);
    static void LookAtBottomRight(IBaseObject baseObject) =>  baseObject.ChangeRotation(Quaternion.Euler(0, 0, 45), false);
    static void LookAtBottom(IBaseObject baseObject) => baseObject.ChangeRotation(Quaternion.Euler(0, 0, 90), false);
    static void LookAtTop(IBaseObject baseObject) =>   baseObject.ChangeRotation(Quaternion.Euler(0, 0, -90), false);


}
