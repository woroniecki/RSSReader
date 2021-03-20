import React from 'react'
import { useHistory, useParams } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import parse from 'html-react-parser'
import { articlesSlice } from 'store/slices'
import { useSelector } from 'react-redux'
import { Button } from 'react-bootstrap'

export interface SingleArticleProps {}

export const SingleArticle: React.FC<SingleArticleProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { id } = useParams<{ id: string }>()
  const { articleid } = useParams<{ articleid: string }>()
  const articlesList = useSelector(articlesSlice.selectAll)

  const renderArticle = () => {
    const numberArticleId = parseInt(articleid)
    if (numberArticleId == NaN) return

    const article = articlesList.find(el => el.id == numberArticleId)
    if (article == null) return

    return (
      <div>
        <a rel="noreferrer" target="_blank" href={article.feedUrl}>
          <Button variant="primary">Visit page</Button>
        </a>
        <h1>{article.title}</h1>
        <div>{parse(article.content)}</div>
      </div>
    )
  }

  return (
    <div style={{ marginTop: 15 }} className="container">
      <Button onClick={() => push(`/blog/${id}`)} variant="primary">
        Return
      </Button>
      {renderArticle()}
    </div>
  )
}

export default SingleArticle
