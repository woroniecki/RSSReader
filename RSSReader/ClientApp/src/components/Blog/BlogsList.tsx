import React from 'react'
import { useSelector } from 'react-redux'
import { useParams } from 'react-router-dom'
import { authSlice, blogsSlice, layoutSlice } from 'store/slices'
import { BlogCard } from './BlogCard'

export interface BlogsListProps {}

export const BlogsList: React.FC<BlogsListProps> = () => {
  const { token } = useSelector(authSlice.stateSelector)
  const blogsList = useSelector(blogsSlice.selectAll)
  const { groupId } = useParams<{ groupId: string }>()
  const { loader } = useSelector(layoutSlice.stateSelector)

  React.useEffect(() => {
    window.scrollTo(0, 0)
  }, [])

  const renderBlogList = () => {
    const noGroupId = parseInt(groupId)

    return blogsList
      .filter(
        el =>
          Number.isNaN(noGroupId) ||
          (el.userData != null && el.userData.groupId == noGroupId) ||
          (el.userData != null &&
            noGroupId == -1 &&
            el.userData.groupId == null)
      )
      .map(el => <BlogCard key={el.id} blog={el} />)
  }

  return <div>{renderBlogList()}</div>
}

export default BlogsList
