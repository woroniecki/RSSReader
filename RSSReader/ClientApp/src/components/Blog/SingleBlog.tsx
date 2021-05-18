import React, { useState } from 'react'
import { Button } from 'react-bootstrap'
import { useSelector } from 'react-redux'
import { useHistory, useParams } from 'react-router-dom'
import { subscriptionsSlice, articlesSlice, authSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import ArticleCard from '../Article/ArticleCard'
import { layoutSlice } from 'store/slices'
import useGetArticles from './../Article/useGetArticles'

export interface SingleBlogProps {}

export const SingleBlog: React.FC<SingleBlogProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { id } = useParams<{ id: string }>()
  const { token } = useSelector(authSlice.stateSelector)
  const subscriptionsList = useSelector(subscriptionsSlice.selectAll)
  const articlesList = useSelector(articlesSlice.selectAll)
  const [isFiltering, setFiltering] = useState(false)

  useGetArticles()

  const getCurrentBlogId = () => {
    const subId = parseInt(id)
    if (subId == NaN) return -1

    const sub = subscriptionsList.find(el => el.id == subId)
    if (sub == null) return -1

    return sub.blog.id
  }

  const enableFilterUnreaded = async () => {
    setFiltering(!isFiltering)
  }
  function getFilterButton(enabled: boolean): string {
    return !enabled ? 'transparent' : ''
  }

  const renderArticles = () => {
    const blogid = getCurrentBlogId()
    if (blogid < 0) return

    return articlesList
      .filter(el => el.blogId == blogid && (isFiltering ? !el.readed : true))
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
      <Button
        className={getFilterButton(isFiltering)}
        onClick={() => {
          enableFilterUnreaded()
        }}
      >
        Filtr readed
      </Button>
      {renderArticles()}
    </div>
  )
}

export default SingleBlog
