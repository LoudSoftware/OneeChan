using System;
using OneeChan.Database.Entities;
using Xunit;

namespace OneeChan.Tests.Database.Entities
{
    public class HouseKeeperTest
    {
        [Fact]
        public void Test1()
        {
            HouseKeeper houseKeeperSettings = new HouseKeeper
            {
                AutoCategoryChannelId = 111111
            };
        }
    }
}