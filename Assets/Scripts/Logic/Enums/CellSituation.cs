
/*

    Hucre durumlarini belirleyen enum yapisi

    empty: ustunde hicbirsey olmayan hucre
    dynamic_full: gecici sureligine dolu olan hucre(ustunde hareket edilebilir)
    static_full: static olarak dolu olan hucre grid yapisina gore ayarlanir
    placed_full: ustunde herhangibir yapi yerlestirilmis hucre tipi
*/

public enum CellSituation  {
    EMPTY, DYNAMIC_FULL, STATIC_FULL, PLACED_FULL, UNKNOWN
}
