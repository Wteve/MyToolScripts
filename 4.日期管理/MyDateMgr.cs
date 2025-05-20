using System.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTool {

    public class MyDateMgr : SingletonMonoBehavior<MyDateMgr> {

        public Dictionary<MyDateType, MyDateInfo> infoDic = new Dictionary<MyDateType, MyDateInfo>();

        public void Init() {
            foreach (MyDateType type in System.Enum.GetValues(typeof(MyDateType))) {
                infoDic.Add(type, new MyDateInfo(type));
            }

            InitMsg();
        }

        public void Clear() {
            ClearMsg();
        }
        public void InitMsg() {

        }

        public void ClearMsg() {

        }

        private void Update() {
            foreach (MyDateInfo info in infoDic.Values) {
                info.OnUpdate();
            }
        }

        public void CheckInterval() {
            foreach (MyDateInfo info in infoDic.Values) {
                info.CheckInterval();
            }
        }

        public MyDateInfo GetInfo(MyDateType type) {
            if (infoDic.ContainsKey(type)) {
                return infoDic[type];
            }
            return null;
        }

        public string GetRefreshTimeStr(MyDateType type) {
            MyDateInfo info = GetInfo(type);

            return GetTimeStr(info.GetNextSeconds());
        }

        public static string GetTimeStr(double seconds) {
            TimeSpan ts = TimeSpan.FromSeconds(seconds);
            return GetTimeStr(ts);
        }

        public static string GetTimeStr(TimeSpan ts) {
            if (ts.Days > 0) {
                return RefLanguage.GetValueParam("{0}d{1}h", ts.Days, ts.Hours); //$"{ts.Days}d {ts.Hours}h";
            }
            if (ts.Hours > 0) {
                return RefLanguage.GetValueParam("{0}h{1}m", ts.Hours, ts.Minutes); //$"{ts.Hours}h {ts.Minutes}m";
            }
            return ts.ToString(@"mm\:ss");
        }

        public void SkipDate(int skipDay) {
            foreach (MyDateInfo info in infoDic.Values) {
                info.SkipDate(skipDay);
            }
        }
    }

    public class MyDateInfo {

        public MyDateType type;

        private string LAST_TIME_KEY = "LAST_TIME";
        private long m_lastTime;
        public long LastTime {
            get => m_lastTime;
            set {
                m_lastTime = value;
                LocalSave.SetString(GetSaveKey(LAST_TIME_KEY), m_lastTime.ToString());
            }
        }
        private DateTime lastDataTime;

        public MyDateInfo(MyDateType _type) {
            type = _type;

            long.TryParse(LocalSave.GetString(GetSaveKey(LAST_TIME_KEY)), out m_lastTime);
            if (m_lastTime == 0) {
                LastTime = TimeMgr.GetTimeStamp(true);
            }
            lastDataTime = TimeMgr.TimeSpanToDateTime(LastTime);

            CheckInterval();
        }

        private string GetSaveKey(string key) {
            return $"MyDate_{type}_{key}";
        }

        private float curTime;
        private float checkTime = 60;
        public void OnUpdate() {
            curTime += Time.deltaTime;
            if (curTime >= checkTime) {
                curTime = 0;
                CheckInterval();
            }
        }

        public void SkipDate(int skipDay) {
            LastTime = TimeMgr.GetTimeStamp(lastDataTime.AddDays(-skipDay), true);
            lastDataTime = TimeMgr.TimeSpanToDateTime(LastTime);

            CheckInterval();
        }

        public void CheckInterval() {
            if (GetNextSeconds() <= 0) {
                LastTime = TimeMgr.GetTimeStamp(true);
                lastDataTime = TimeMgr.TimeSpanToDateTime(LastTime);

                Send.SendMsg(SendType.NewDate, type);
            }
        }

        private DateTime GetWeekData(DateTime dateTime) {
            int lastDayOfWeek = (int)lastDataTime.Date.DayOfWeek;
            //以周一为基准
            int offset = (int)DayOfWeek.Monday - lastDayOfWeek;
            if (offset != 0) {
                return dateTime.Date.AddDays(offset);
            }
            return dateTime;
        }

        public double GetRefreshSeconds() {
            switch (type) {
                case MyDateType.Daily:
                    return 1d * 24 * 60 * 60;
                case MyDateType.Weekly:
                    return 7d * 24 * 60 * 60;
            }
            return 0;
        }

        public double GetNextSeconds() {
            switch (type) {
                case MyDateType.Daily:
                    return GetRefreshSeconds() - DateTime.UtcNow.Subtract(lastDataTime.Date).TotalSeconds;
                case MyDateType.Weekly:
                    DateTime lastWeekDate = GetWeekData(lastDataTime.Date);
                    return GetRefreshSeconds() - DateTime.UtcNow.Subtract(lastWeekDate.Date).TotalSeconds;
            }
            return 0;
        }

    }

    public enum MyDateType {
        Daily,
        Weekly,
    }

}