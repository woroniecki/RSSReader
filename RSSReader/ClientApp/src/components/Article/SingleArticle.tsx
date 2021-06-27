import React from 'react'
import { useHistory, useParams } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import parse from 'html-react-parser'
import { articlesSlice } from 'store/slices'
import { useSelector } from 'react-redux'
import { authSlice } from 'store/slices'
import ArticlePatchButtons from './ArticlePatchButtons'
import useGetArticles from './useGetArticles'
import { Button, CardHeader, Divider, Typography } from '@material-ui/core'

export interface SingleArticleProps {}

export const SingleArticle: React.FC<SingleArticleProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { id } = useParams<{ id: string }>()
  const { articleid } = useParams<{ articleid: string }>()
  const articlesList = useSelector(articlesSlice.selectAll)
  const { token } = useSelector(authSlice.stateSelector)

  useGetArticles()

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
    const article = articlesList.find(el => el.id == parseInt(articleid))

    if (article == null) return

    readPostRequest()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token])

  const renderVisitPageButton = (url: string) => {
    if (!url || url.length === 0) return
    return (
      <a rel="noreferrer" target="_blank" href={url}>
        <Button>Visit page</Button>
      </a>
    )
  }

  const renderArticle = () => {
    const numberArticleId = parseInt(articleid)
    if (numberArticleId == NaN) return

    const article = articlesList.find(el => el.id == numberArticleId)
    if (article == null) return

    return (
      <>
        <CardHeader
          action={
            <>
              <ArticlePatchButtons
                blogId={article.blogId}
                postId={article.id}
                readed={article.readed}
                favourite={article.favourite}
              />
              {renderVisitPageButton(article.url)}
              <Button onClick={() => push(`/blog/${id}`)}>Return</Button>
            </>
          }
          title={<Typography variant="h4">{article.name}</Typography>}
          subheader={`${article.publishDate}, ${article.author}`}
        />
        <Divider />

        {parse(article.content)}
      </>
    )
  }

  return <>{renderArticle()}</>
}

export default SingleArticle
