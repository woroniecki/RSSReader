import React from 'react'
import { useHistory, useParams } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import parse from 'html-react-parser'
import { articlesSlice } from 'store/slices'
import { useSelector } from 'react-redux'
import { Button } from 'react-bootstrap'
import { authSlice } from 'store/slices'
import ArticlePatchButtons from './ArticlePatchButtons'

export interface SingleArticleProps {}

export const SingleArticle: React.FC<SingleArticleProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { id } = useParams<{ id: string }>()
  const { articleid } = useParams<{ articleid: string }>()
  const articlesList = useSelector(articlesSlice.selectAll)
  const { token } = useSelector(authSlice.stateSelector)

  const readPostRequest = async () => {
    const promise = await dispatch(
      articlesSlice.putReadArticle({
        blogId: parseInt(id),
        postId: parseInt(articleid),
      })
    )

    if (articlesSlice.getArticles.fulfilled.match(promise)) {
    } else {
    }
  }

  React.useEffect(() => {
    if (token) {
      readPostRequest()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token])

  const renderArticle = () => {
    const numberArticleId = parseInt(articleid)
    if (numberArticleId == NaN) return

    const article = articlesList.find(el => el.id == numberArticleId)
    if (article == null) return

    return (
      <div>
        <Button onClick={() => push(`/blog/${id}`)} variant="primary">
          Return
        </Button>
        <a rel="noreferrer" target="_blank" href={article.feedUrl}>
          <Button variant="primary">Visit page</Button>
        </a>
        <ArticlePatchButtons
          blogId={article.blogId}
          postId={article.id}
          readed={article.readed}
          favourite={article.favourite}
        />
        <h1>{article.name}</h1>
        <div>{parse(article.content)}</div>
      </div>
    )
  }

  return (
    <div style={{ marginTop: 15 }} className="container">
      {renderArticle()}
    </div>
  )
}

export default SingleArticle
