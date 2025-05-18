using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Video_Library_Api.Exceptions;
using Video_Library_Api.Models;
using Video_Library_Api.Paging;
using Video_Library_Api.Repositories;
using Video_Library_Api.Resources;

namespace Video_Library_Api.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TagService(ITagRepository tagRepository, IUnitOfWork unitOfWork)
        {
            _tagRepository = tagRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Tag> DeleteAsync(string name)
        {
            Tag tag = await _tagRepository.FindByNameAsync(name);

            if(tag == null)
            {
                throw new NotFoundException("Tag not found");
            }

            if(!tag.UserAdded)
            {
                throw new NotUserAddedTagException("Can't delete builtin tag");
            }

            try
            {
                _tagRepository.Remove(tag);
                await _unitOfWork.CompleteAsync();

                return tag;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Tag> FindByNameAsync(string name)
        {
            Tag tag = null;
            tag = await _tagRepository.FindByNameAsync(name);

            return tag;
        }

        public async Task<Tag> FindAsync(int id)
        {
            Tag tag = await _tagRepository.FindByIdAsync(id);

            if(tag == null)
            {
                throw new NotFoundException("Tag not found");
            }

            return tag;
        }


        public async Task<IEnumerable<Tag>> ListAsync()
        {
            return await _tagRepository.ListAsync();
        }

        public async Task<Tag> SaveAsync(Tag tag)
        {
            tag.UserAdded = true;
            if(await _tagRepository.FindByNameAsync(tag.Name) != null)
            {
                throw new TagExistsException("Tag already exists");
            }
            
            tag = _tagRepository.Add(tag);
            await _unitOfWork.CompleteAsync();
            return tag;
        }

        public async Task<Tag> UpdateAsync(string name, Tag tag)
        {
            // just placeholder code            
            Tag oldTag = await _tagRepository.FindByNameAsync(name);

            if(oldTag == null)
            {
                throw new NotFoundException("Tag not found");
            }

            if(!oldTag.UserAdded)
            {
                throw new NotUserAddedTagException("Can't change builtin tag");
            }

            oldTag.Name = tag.Name;

            _tagRepository.Update(oldTag);
            await _unitOfWork.CompleteAsync();

            return oldTag;
        }

        public async Task<PaginatedList<VideosTags>> ListAsyncByVideoId(string videoId, PagingParams pagingParams)
        {               
            return await _tagRepository.ListAsyncByVideoId(videoId, pagingParams);
        }


        public async Task<VideosTagsResource> DeleteVideoTagsAsync(string videoId,string name, int? from, int? to)
        {
            var vty = await _tagRepository.DeleteVideoTagsAsync(videoId, name, from, to);
            await _unitOfWork.CompleteAsync();
            return vty;
        }
    }
}