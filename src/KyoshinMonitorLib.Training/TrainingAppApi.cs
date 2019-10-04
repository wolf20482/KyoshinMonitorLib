﻿using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using KyoshinMonitorLib.ApiResult.AppApi;
using KyoshinMonitorLib.UrlGenerator;

namespace KyoshinMonitorLib.Training
{
	/// <summary>
	/// トレーニング用API
	/// </summary>
	public class TrainingAppApi : AppApi
	{
		/// <summary>
		/// ベースディレクトリ
		/// </summary>
		public string BasePath { get; }
		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="basePath">かならずディレクトリ区切り文字で終わらせること！</param>
		/// <param name="points">観測点一覧</param>
		public TrainingAppApi(string basePath, ObservationPoint[] points) : base(points)
		{
			BasePath = basePath;
		}

		/// <summary>
		/// 観測点一覧
		/// </summary>
		public override Task<ApiResult<SiteList>> GetSiteList(string baseSerialNo)
		{
			var path = AppApiUrlGenerator.Generate(baseSerialNo).Replace("http://ts.qtmoni.bosai.go.jp/qt/tsapp/kyoshin_monitor/static/sip_data/", BasePath);
			if (!File.Exists(path))
				return Task.FromResult(new ApiResult<SiteList>(HttpStatusCode.NotFound, null));
			return Task.FromResult(new ApiResult<SiteList>(HttpStatusCode.OK, JsonSerializer.Deserialize<SiteList>(File.ReadAllText(path))));
		}

		/// <summary>
		/// 観測データ
		/// </summary>
		public override Task<ApiResult<RealTimeData>> GetRealTimeData(DateTime time, RealTimeDataType dataType, bool isBehore = false)
		{ 
			var path = AppApiUrlGenerator.Generate(AppApiUrlType.RealTimeData, time, dataType, isBehore).Replace("http://ts.qtmoni.bosai.go.jp/qt/tsapp/kyoshin_monitor/static/sip_data/", BasePath);
			if (!File.Exists(path))
				return Task.FromResult(new ApiResult<RealTimeData>(HttpStatusCode.NotFound, null));
			return Task.FromResult(new ApiResult<RealTimeData>(HttpStatusCode.OK, JsonSerializer.Deserialize<RealTimeData>(File.ReadAllText(path))));
		}

		/// <summary>
		/// 緊急地震速報データ
		/// </summary>
		[Obsolete]
		public override Task<ApiResult<Hypo>> GetEewHypoInfo(DateTime time)
		{
			var path = AppApiUrlGenerator.Generate(AppApiUrlType.HypoInfoJson, time).Replace("http://kv.kmoni.bosai.go.jp/kyoshin_monitor/static/jsondata/", BasePath);
			if (!File.Exists(path))
				return Task.FromResult(new ApiResult<Hypo>(HttpStatusCode.NotFound, null));
			return Task.FromResult(new ApiResult<Hypo>(HttpStatusCode.OK, JsonSerializer.Deserialize<Hypo>(File.ReadAllText(path))));
		}
	}
}
