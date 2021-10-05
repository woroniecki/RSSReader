﻿using AutoMapper;
using DataLayer.Code;
using DataLayer.Models;
using DbAccess.Core;
using LogicLayer.Blogs;
using LogicLayer.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RSSReader.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogicLayer.PopulateDB
{
    public class Seed
    {
        public static void SeedBlogs(
            IHost host,
            ILogger<Seed> logger)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<DataContext>();
                context.Database.Migrate();
                if (context.Blogs.Any())
                {
                    return;
                }
            }

            var mapper_conf = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles());
            });
            var mapper = mapper_conf.CreateMapper();
            IHttpHelperService httpService = new HttpHelperService();

            logger.LogInformation("Run blogs populating");
            string path = Directory.GetCurrentDirectory() + "/../GetFeedsList/list.txt";

            using (StreamReader sw = new StreamReader(path))
            {
                string url;
                List<string> urls = new List<string>();
                List<string> failed_add = new List<string>();
                List<string> succeed_add = new List<string>();

                while ((url = sw.ReadLine()) != null)
                {
                    urls.Add(url);
                }

                var batchSize = 10;
                int batchCount = (int)Math.Ceiling((double)urls.Count / batchSize);

                for (int i = 0; i < batchCount; i++)
                {
                    Console.WriteLine($"New batch run {i}/{batchCount}");

                    var urlsToAddBlog = urls.Skip(i * batchSize).Take(batchSize);
                    var tasks = urlsToAddBlog.Select(
                        url => AddBlogTask(
                            host,
                            logger,
                            mapper,
                            httpService,
                            url,
                            succeed_add,
                            failed_add)
                        ).ToArray();

                    Task.WaitAll(tasks);

                    //context.SaveChanges();
                    Console.WriteLine($"Batch run end");
                }

                logger.LogInformation($"Populating blogs finished total={urls.Count}");

                string succeed_str = "SUCCEED\n";
                foreach (var str in succeed_add)
                {
                    succeed_str += $"    {str}\n";
                }

                string failed_str = "FAILED\n";
                foreach (var str in failed_add)
                {
                    failed_str += $"    {str}\n";
                }

                logger.LogInformation(succeed_str);
                logger.LogInformation(failed_str);
                logger.LogInformation($"SUCCEED: {succeed_add.Count} FAILED: {failed_add.Count}");

                sw.Close();
            }
        }

        private static async Task<bool> AddBlogTask(
            IHost host,
            ILogger<Seed> logger,
            IMapper mapper,
            IHttpHelperService httpService,
            string url,
            List<string> succeed,
            List<string> failed)
        {
            try
            {
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var context = services.GetRequiredService<DataContext>();
                    var uow = new UnitOfWork(context);

                    GetOrCreateBlogAction getOrCreateBlogAction = new GetOrCreateBlogAction(httpService, uow, mapper);

                    Blog blog = await getOrCreateBlogAction.ActionAsync(url);

                    if (blog == null || getOrCreateBlogAction.HasErrors)
                    {
                        logger.LogError($"Failed to add blog {url}\n{getOrCreateBlogAction.Errors[0]}");

                        lock (failed)
                        {
                            failed.Add($"{url} - {getOrCreateBlogAction.Errors[0]}");
                        }

                        return false;
                    }

                    lock (succeed)
                    {
                        succeed.Add(url);
                    }

                    await context.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to add blog {url}\n{ex}");
            }

            lock (failed)
            {
                failed.Add(url);
            }

            return false;

        }
    }
}