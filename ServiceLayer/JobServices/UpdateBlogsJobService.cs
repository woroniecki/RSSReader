using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Code;
using DataLayer.Models;
using Dtos.Jobs;
using LogicLayer.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Parsers.Rss;

namespace ServiceLayer.JobServices
{
    public class UpdateBlogsJobService : IUpdateBlogsJobService
    {
        //private IAction _action;
        private IMapper _mapper;
        private IHttpHelperService _httpService;
        private DataContext _context;
        private ILogger<UpdateBlogsJobService> _logger;

        //public IImmutableList<ValidationResult> Errors => _action != null ? _action.Errors : null;

        public UpdateBlogsJobService(
            IMapper mapper,
            IHttpHelperService httpService,
            DataContext context,
            ILogger<UpdateBlogsJobService> logger)
        {
            _mapper = mapper;
            _httpService = httpService;
            _context = context;
            _logger = logger;
        }

        public async Task<UpdateBlogsJobResponse> UpdateBlogs()
        {
            List<string> failed_updates = new List<string>();
            List<string> succeeded_updates = new List<string>();
            List<string> no_update = new List<string>();
            int big_step = 32;
            int small_step = 4;
            int skip_amount = 0;

            List<Blog> blogs;
            do
            {
                blogs = await _context.Blogs
                                      .Include(x => x.Posts)
                                      .OrderBy(x => x.Id)
                                      .Skip(skip_amount).Take(big_step)
                                      .ToListAsync();

                _logger.LogInformation($"UpdateBlogs progress: {skip_amount} updated");

                skip_amount += big_step;

                for (int i = 0; i < blogs.Count; i += small_step)
                {
                    List<Task> tasks = new List<Task>(small_step + 1);

                    for (int j = i; j < i + small_step && j < blogs.Count; j++)
                    {
                        Blog blog = blogs[j];
                        tasks.Add(UpdateBlog(blog, failed_updates, succeeded_updates, no_update));
                    }

                    await Task.WhenAll(tasks);

                    await _context.SaveChangesAsync();
                }
            }
            while (blogs.Count() > 0);

            var result = new UpdateBlogsJobResponse()
            {
                Failed = failed_updates,
                Succeeded = succeeded_updates,
                NoUpdate = no_update
            };

            return result;
        }

        private async Task UpdateBlog(Blog blog, List<string> failed_updates, List<string> succeeded_updates, List<string> no_update)
        {
            try
            {
                string content = await _httpService.GetStringContent(blog.Url);
                IEnumerable < RssSchema > rss_schemas = FeedMethods.Parse(content);
                var result = blog.UpdatePosts(rss_schemas, _mapper);
                if (result == null)
                {
                    lock (failed_updates)
                    {
                        failed_updates.Add($"{blog.Url}");
                    }
                }
                else
                {
                    if (result.Value.Added > 0 || result.Value.Deleted > 0)
                    {
                        lock (succeeded_updates)
                        {
                            succeeded_updates.Add($"{blog.Url} - {result}");
                        }
                    }
                    else
                    {
                        lock (no_update)
                        {
                            no_update.Add($"{blog.Url}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lock (failed_updates)
                {
                    failed_updates.Add($"{blog.Url} - {ex.Message}");
                }
            }
        }
    }

    public interface IUpdateBlogsJobService {
        Task<UpdateBlogsJobResponse> UpdateBlogs();
    }
}
