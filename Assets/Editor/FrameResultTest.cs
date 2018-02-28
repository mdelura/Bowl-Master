using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class FrameResultTest
{
    GameManager _scoreManager;

    [Test]
    public void Bonus_ReturnsExpectedValue()
    {
        //Arrange
        var items = new Tuple<FrameResult, Bonus>[]
        {
            new Tuple<FrameResult, Bonus>(new FrameResult(new int[] { 1, 1 }), Bonus.None),
            new Tuple<FrameResult, Bonus>(new FrameResult(new int[] { 10 }), Bonus.Strike),
            new Tuple<FrameResult, Bonus>(new FrameResult(new int[] { 1, 9 }), Bonus.Spare),
            new Tuple<FrameResult, Bonus>(new FrameResult(new int[] { 10, 1, 1 }), Bonus.Strike),
            new Tuple<FrameResult, Bonus>(new FrameResult(new int[] { 10, 10, 10 }), Bonus.Strike),
            new Tuple<FrameResult, Bonus>(new FrameResult(new int[] { 1, 9, 1 }), Bonus.Spare),
            new Tuple<FrameResult, Bonus>(new FrameResult(new int[] { 1, 10, 1 }), Bonus.Spare),
        }
        .Select(t => new
        {
            FrameResult = t.Item1,
            ExpectedBonus = t.Item2
        })
        .ToArray();

        //Assert
        CollectionAssert.AreEqual(items.Select(i => i.ExpectedBonus), items.Select(i => i.FrameResult.Bonus));
    }



}