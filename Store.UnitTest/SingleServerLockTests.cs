﻿using System;
using System.Collections.Generic;

using StackExchange.Redis;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Store.Redlock;

namespace Store.UnitTest
{
    [TestClass()]
    public class SingleServerLockTests
    {
        private const string resourceName = "MyResourceName";
        private List<Process> redisProcessList = new List<Process>();
        [TestMethod()]
        public void setup()
        {
            // Launch Server
            Process redis = new Process();

            // Configure the process using the StartInfo properties.
            redis.StartInfo.FileName = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"..\..\..\packages\Redis-32.2.6.12.1\tools\redis-server.exe");
            redis.StartInfo.Arguments = "--port 6379";
            redis.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            redis.Start();

            redisProcessList.Add(redis);
        }

        [TestMethod()]
        public void teardown()
        {
            foreach (var process in redisProcessList)
            {
                if (!process.HasExited) process.Kill();
            }

            redisProcessList.Clear();
        }
        [TestMethod()]
        public void TestInsertRedis()
        {
            var redis = new RedLock(ConnectionMultiplexer.Connect("127.0.0.1:6379"));
            var client = redis.redisMasterDictionary["127.0.0.1:6379"];

            var succeeded = client.GetDatabase().StringSet("testkey", "21232", TimeSpan.FromDays(1), When.NotExists);
            Assert.IsTrue(succeeded);

        }

        [TestMethod()]
        public void TestWhenLockedAnotherLockRequestIsRejected()
        {
            var dlm = new RedLock(ConnectionMultiplexer.Connect("127.0.0.1:6379"));

            Lock lockObject;
            Lock newLockObject;
            var locked = dlm.Lock(resourceName, new TimeSpan(0, 0, 10), out lockObject);
            Assert.IsTrue(locked, "Unable to get lock");
            var locked2 = dlm.Lock(resourceName, new TimeSpan(0, 0, 10), out newLockObject);
            Assert.IsFalse(locked2, "lock taken, it shouldn't be possible");
            dlm.Unlock(lockObject);
        }

        [TestMethod()]
        public void TestThatSequenceLockedUnlockedAndLockedAgainIsSuccessfull()
        {
            var dlm = new RedLock(ConnectionMultiplexer.Connect("127.0.0.1:6379"));

            Lock lockObject;
            Lock newLockObject;
            var locked = dlm.Lock(resourceName, new TimeSpan(0, 0, 10), out lockObject);
            Assert.IsTrue(locked, "Unable to get lock");
            dlm.Unlock(lockObject);
            locked = dlm.Lock(resourceName, new TimeSpan(0, 0, 10), out newLockObject);
            Assert.IsTrue(locked, "Unable to get lock");
            dlm.Unlock(newLockObject);
        }

    }

}
