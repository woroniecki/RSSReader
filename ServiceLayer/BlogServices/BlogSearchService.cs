using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Models;
using DbAccess.Core;
using Dtos.Blogs;
using Dtos.Subscriptions;
using LogicLayer._const;
using ServiceLayer._Commons;

namespace ServiceLayer.BlogServices
{
    public class BlogSearchService : IBlogSearchService
    {
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;

        public BlogSearchService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SearchBlogResponseDto>> Search(string value)
        {
            var result_list = await _unitOfWork.BlogRepo.Search(value, 5);

            return _mapper.Map<IEnumerable<Blog>, IEnumerable<SearchBlogResponseDto>>(result_list);
        }
    }

    public interface IBlogSearchService
    {
        Task<IEnumerable<SearchBlogResponseDto>> Search(string value);
    }
}
