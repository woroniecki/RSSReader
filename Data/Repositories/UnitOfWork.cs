using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSSReader.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        DataContext _context;
        
        ReaderRepository _readerRepository;

        BlogRepository _blogRepository;
        GroupRepository _groupRepository;
        PostRepository _postRepository;
        SubscriptionRepository _subscriptionRepository;
        UserPostDataRepository _userPostDataRepository;
        UserRepository _userRepository;

        public UnitOfWork(DataContext context)
        {
            _context = context;
        }

        public IReaderRepository ReaderRepo
        {
            get
            {
                if (this._readerRepository == null)
                {
                    this._readerRepository = new ReaderRepository(_context);
                }
                return _readerRepository;
            }
        }

        public IBlogRepository BlogRepo
        {
            get
            {
                if (this._blogRepository == null)
                {
                    this._blogRepository = new BlogRepository(_context);
                }
                return _blogRepository;
            }
        }

        public IGroupRepository GroupRepo
        {
            get
            {
                if (this._groupRepository == null)
                {
                    this._groupRepository = new GroupRepository(_context);
                }
                return _groupRepository;
            }
        }

        public IPostRepository PostRepo
        {
            get
            {
                if (this._postRepository == null)
                {
                    this._postRepository = new PostRepository(_context);
                }
                return _postRepository;
            }
        }

        public ISubscriptionRepository SubscriptionRepo
        {
            get
            {
                if (this._subscriptionRepository == null)
                {
                    this._subscriptionRepository = new SubscriptionRepository(_context);
                }
                return _subscriptionRepository;
            }
        }

        public IUserPostDataRepository UserPostDataRepo
        {
            get
            {
                if (this._userPostDataRepository == null)
                {
                    this._userPostDataRepository = new UserPostDataRepository(_context);
                }
                return _userPostDataRepository;
            }
        }

        public IUserRepository UserRepo
        {
            get
            {
                if (this._userRepository == null)
                {
                    this._userRepository = new UserRepository(_context);
                }
                return _userRepository;
            }
        }

        #region Disposing

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    public interface IUnitOfWork : IDisposable
    {
        IReaderRepository ReaderRepo { get; }
        IBlogRepository BlogRepo { get; }
        IGroupRepository GroupRepo { get; }
        IPostRepository PostRepo { get; }
        ISubscriptionRepository SubscriptionRepo { get; }
        IUserPostDataRepository UserPostDataRepo { get; }
        IUserRepository UserRepo { get; }
    }
}
