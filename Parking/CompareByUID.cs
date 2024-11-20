public class CompareByUID : IComparer<IParkingSpace>
{
    public int Compare(IParkingSpace? x, IParkingSpace? y)
    {
        ValueTuple<int, int> res;
        ValueTuple<int, int> res1;
        if(x is not null && y is not null)
        {
            res = UIDToPos.UIdToPos(x.UId);
            res1 = UIDToPos.UIdToPos(y.UId);
            if(res1.Item2 - res.Item2 == 0)
            {
                return (res1.Item1 - res.Item1);
            }
            return res1.Item2 - res.Item2;
        }
        return string.Compare(x.UId, y.UId);

    }
}
