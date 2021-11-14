import { Button, CardHeader, Divider, Typography } from '@material-ui/core'
import React from 'react'
import { useSelector } from 'react-redux'
import { useHistory, useParams } from 'react-router-dom'
import { articlesSlice, blogsSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import ArticleCard from '../Article/ArticleCard'
import useGetArticles from './../Article/useGetArticles'
import BlogAvatar from './BlogAvatar'
import BlogGroup from './BlogGroup'

export interface SingleBlogProps {}

export const SingleBlog: React.FC<SingleBlogProps> = () => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { blogid } = useParams<{ blogid: string }>()
  const blogsList = useSelector(blogsSlice.selectAll)
  const articlesList = useSelector(articlesSlice.selectAll)

  useGetArticles()

  const patchSubscription = async (subId: number, filterReaded: boolean) => {
    const promise = await dispatch(
      blogsSlice.patchSubscription({
        subId: subId,
        filterReaded: filterReaded,
      })
    )

    if (articlesSlice.patchPost.fulfilled.match(promise)) {
    } else {
    }
  }

  const getCurrentBlog = () => {
    const blogId = parseInt(blogid)
    if (blogId == NaN) return null

    const blog = blogsList.find(el => el.id == blogId)
    return blog
  }

  const renderHeader = () => {
    const blog = getCurrentBlog()
    if (blog == null) return
    return (
      <>
        <CardHeader
          avatar={<BlogAvatar title={blog.name} imageUrl={blog.imageUrl} />}
          action={
            <>
              <Button
                className={getFilterButtonClass(blog.userData.filterReaded)}
                onClick={() => {
                  patchSubscription(
                    blog.userData.subId,
                    !blog.userData.filterReaded
                  )
                }}
              >
                Filtr readed
              </Button>
              <Button onClick={() => push('/')}>Return</Button>
            </>
          }
          title={<Typography variant="h4">{blog.name}</Typography>}
          subheader={
            <BlogGroup
              subId={blog.userData.subId}
              activeGroupId={blog.userData.groupId}
            />
          }
        />
      </>
    )
  }

  const renderArticles = () => {
    const blog = getCurrentBlog()
    if (blog == null) return

    const blogId = blog.id
    const filterReaded = blog.userData != null && blog.userData.filterReaded

    return articlesList
      .filter(
        el =>
          el.blogId == blogId &&
          (filterReaded ? el.userData != null && !el.userData.readed : true)
      )
      .sort((a, b) => {
        return (
          new Date(b.publishDate).getTime() - new Date(a.publishDate).getTime()
        )
      })
      .map(el => (
        <ArticleCard
          key={el.id}
          articleid={el.id}
          title={el.name}
          summary={el.summary}
          content={el.content}
          url={el.url}
          imageUrl={el.imageUrl}
          publishDate={el.publishDate}
          userData={el.userData}
        />
      ))
  }

  function getFilterButtonClass(enabled: boolean): string {
    return !enabled ? 'transparent' : ''
  }

  return (
    <div style={{ marginTop: 15 }} className="container">
      {renderHeader()}
      <Divider />
      {renderArticles()}
    </div>
  )
}

export default SingleBlog
