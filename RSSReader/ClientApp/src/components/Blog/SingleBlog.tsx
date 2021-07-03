import { Button, CardHeader, Divider, Typography } from '@material-ui/core'
import React, { useState } from 'react'
import { useSelector } from 'react-redux'
import { useHistory, useParams } from 'react-router-dom'
import {
  subscriptionsSlice,
  blogsSlice,
  articlesSlice,
  authSlice,
} from 'store/slices'
import { useAppDispatch } from 'store/store'
import ArticleCard from '../Article/ArticleCard'
import useGetArticles from './../Article/useGetArticles'
import BlogAvatar from './BlogAvatar'
import BlogGroup from './BlogGroup'

export interface SingleBlogProps {}

export const SingleBlog: React.FC<SingleBlogProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { blogid } = useParams<{ blogid: string }>()
  const { token } = useSelector(authSlice.stateSelector)
  const blogsList = useSelector(blogsSlice.selectAll)
  const articlesList = useSelector(articlesSlice.selectAll)

  useGetArticles()

  const patchSubscription = async (subId: number, filterReaded: boolean) => {
    const promise = await dispatch(
      subscriptionsSlice.patchSubscription({
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
          avatar={<BlogAvatar title={blog.name} imageUrl={blog.url} />}
          action={
            <>
              <Button
                className={getFilterButtonClass(blog.userData.filterReaded)}
                onClick={() => {
                  patchSubscription(blog.id, !blog.userData.filterReaded)
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
    console.log(blog.id)

    const blogId = blog.id
    const filterReaded = blog.userData != null && blog.userData.filterReaded

    return articlesList
      .filter(el => el.blogId == blogId && (filterReaded ? !el.readed : true))
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
          readed={el.readed}
          favourite={el.favourite}
          publishDate={el.publishDate}
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
