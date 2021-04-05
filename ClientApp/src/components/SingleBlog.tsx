import { Post } from 'api/api.types'
import React, { useState } from 'react'
import { Button } from 'react-bootstrap'
import { useSelector } from 'react-redux'
import { useHistory, useParams } from 'react-router-dom'
import { subscriptionsSlice, articlesSlice, authSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import * as blogApi from '../api/blogApi'
import ArticleCard from './AppHome/ArticleCard'
import { layoutSlice } from 'store/slices'

export interface SingleBlogProps {}

type CustomFeed = { foo: string }
type CustomItem = { bar: number }

export const SingleBlog: React.FC<SingleBlogProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { id } = useParams<{ id: string }>()
  const { token } = useSelector(authSlice.stateSelector)
  const subscriptionsList = useSelector(subscriptionsSlice.selectAll)
  const articlesList = useSelector(articlesSlice.selectAll)

  const getCurrentBlogId = () => {
    const subId = parseInt(id)
    if (subId == NaN) return -1

    const sub = subscriptionsList.find(el => el.id == subId)
    if (sub == null) return -1

    return sub.blog.id
  }

  const fetchList = async () => {
    const blogid = getCurrentBlogId()
    if (blogid < 0) return

    const list_already_taken = articlesList.find(el => el.blogId == blogid)
    if (list_already_taken != null) return

    dispatch(layoutSlice.actions.setLoader(true))

    const promise = await dispatch(articlesSlice.getArticles(blogid))

    if (articlesSlice.getArticles.fulfilled.match(promise)) {
    } else {
    }

    dispatch(layoutSlice.actions.setLoader(false))
  }
  React.useEffect(() => {
    if (token) {
      fetchList()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token])

  const renderArticles = () => {
    const blogid = getCurrentBlogId()
    if (blogid < 0) return

    return articlesList
      .filter(el => el.blogId == blogid)
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
          url={el.feedUrl}
          imageUrl={el.imageUrl}
          readed={el.readed}
          favourite={el.favourite}
          publishDate={el.publishDate}
        />
      ))
  }

  return (
    <div style={{ marginTop: 15 }} className="container">
      <Button onClick={() => push('/')} variant="primary">
        Return
      </Button>
      {renderArticles()}
    </div>
  )
}

export default SingleBlog
