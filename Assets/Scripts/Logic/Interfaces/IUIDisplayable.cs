
/*
    UI elementinin logic kismindan ihtiyaci olan verileri belirleyen interface
*/

public interface IUIDisplayable {
    public string Name { get; }
    public delegate string StringReturningMethod();
    public StringReturningMethod GetInfoMessage();  // mesajin dinamik olarak guncellenmesini saglayan fonksiyon
    public ObjectType ObjectType { get; }
}
