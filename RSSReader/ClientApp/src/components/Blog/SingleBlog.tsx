import { Button, CardHeader, Divider, Typography } from '@material-ui/core'
import React, { useState } from 'react'
import { useSelector } from 'react-redux'
import { useHistory, useParams } from 'react-router-dom'
import { subscriptionsSlice, articlesSlice, authSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import ArticleCard from '../Article/ArticleCard'
import useGetArticles from './../Article/useGetArticles'
import BlogAvatar from './BlogAvatar'
import BlogGroup from './BlogGroup'

export interface SingleBlogProps {}

export const SingleBlog: React.FC<SingleBlogProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { id } = useParams<{ id: string }>()
  const { token } = useSelector(authSlice.stateSelector)
  const subscriptionsList = useSelector(subscriptionsSlice.selectAll)
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

  const getCurrentSub = () => {
    const subId = parseInt(id)
    if (subId == NaN) return null

    const sub = subscriptionsList.find(el => el.id == subId)
    return sub
  }

  const renderHeader = () => {
    const sub = getCurrentSub()
    if (sub == null) return
    return (
      <>
        <CardHeader
          avatar={<BlogAvatar title={sub.blog.name} imageUrl={sub.blog.url} />}
          action={
            <>
              <Button
                className={getFilterButtonClass(sub.filterReaded)}
                onClick={() => {
                  patchSubscription(sub.id, !sub.filterReaded)
                }}
              >
                Filtr readed
              </Button>
              <Button onClick={() => push('/')}>Return</Button>
            </>
          }
          title={<Typography variant="h4">{sub.blog.name}</Typography>}
          subheader={<BlogGroup subId={sub.id} activeGroupId={sub.groupId} />}
        />
      </>
    )
  }

  const renderArticles = () => {
    const sub = getCurrentSub()
    if (sub == null) return

    const blogid = sub.blog.id

    return articlesList
      .filter(
        el => el.blogId == blogid && (sub.filterReaded ? !el.readed : true)
      )
      .sort((a, b) => {
        return (
          new Date(b.publishDate).getTime() - new Date(a.publishDate).getTime()
        )
      })
      .map(el => (
        <ArticleCard
          key={el.id}
          id={el.id}
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
