public interface IParkingLotApp
{
    IRenderer Render { get; }
    IList<IViewModel> Models { get; }
    string[] MessageQue { get; }
    IParkManagerEngine Engine { get; }
    public int Start();
}
//create some sort of way to communicate to messagebuffer with
