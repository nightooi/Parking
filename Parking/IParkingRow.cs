﻿using System.Runtime.CompilerServices;
using System.Security.Cryptography;
// this should inherit from IEnumberable<IParking> and IEnumerator<IParking>
// hopefully we'll be able to implement that.
//
public interface IParkingRow : IReadOnlyCollection<IParkingSpace>
{
    protected string RowEnumeration { get; }
    //0 iterated counted from left to right
    bool HasOpposing { get; }
    int RowNumber { get; }
    int OpposingRow { get; }
    //0 Iterated
    int TotalSpaces { get; }
    public bool IsTaken(int position);
    public IParkingSpace GetParking(int position);
}
