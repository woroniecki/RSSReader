import { Post } from 'api/api.types'
import React, { useState } from 'react'
import { Button } from 'react-bootstrap'
import { useSelector } from 'react-redux'
import { useHistory, useParams } from 'react-router-dom'
import { subscriptionsSlice, articlesSlice, authSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import * as blogApi from '../api/blogApi'
import ArticleCard from './AppHome/ArticleCard'

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

  const fetchList = async () => {
    const subId = parseInt(id)
    if (subId == NaN) return

    const sub = subscriptionsList.find(el => el.id == subId)
    if (sub == null) return

    const promise = await dispatch(articlesSlice.getArticles(sub.blog.id))

    if (articlesSlice.getArticles.fulfilled.match(promise)) {
    } else {
    }
  }
  React.useEffect(() => {
    if (token) {
      fetchList()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token])

  const renderArticles = () => {
    return articlesList.map(el => (
      <ArticleCard
        key={el.id}
        id={el.id}
        title={el.title}
        summary={el.summary}
        content={el.content}
        url={el.feedUrl}
        imageUrl={el.imageUrl}
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
