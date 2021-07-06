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
import styled from 'styled-components'
import AppButton from 'components/layout/AppButton'

const SingleArticleWrapper = styled.div`
  img {
    max-width: 100%;
    width: 100%;
    height: auto;
  }
  pre {
    box-sizing: border-box;
    width: 100%;
    padding: 0;
    margin: 0;
    overflow: auto;
    overflow-y: hidden;
    font-size: 12px;
    line-height: 20px;
    background: #efefef;
    border: 1px solid #777;
    background: url(lines.png) repeat 0 0;
    padding: 10px;
    color: #333;
    overflow-x: auto;
    white-space: pre-wrap;
  }
`
export interface SingleArticleProps {}

export const SingleArticle: React.FC<SingleArticleProps> = () => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { blogid } = useParams<{ blogid: string }>()
  const { articleid } = useParams<{ articleid: string }>()
  const articlesList = useSelector(articlesSlice.selectAll)
  const { token } = useSelector(authSlice.stateSelector)

  useGetArticles()

  const readPostRequest = async () => {
    const promise = await dispatch(
      articlesSlice.putReadArticle({
        blogId: parseInt(blogid),
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
        <AppButton weight={700}>Visit page</AppButton>
      </a>
    )
  }

  const renderArticle = () => {
    const numberArticleId = parseInt(articleid)
    if (numberArticleId == NaN) return '404'

    const article = articlesList.find(el => el.id == numberArticleId)
    if (article == null) return '404'

    return (
      <>
        <CardHeader
          action={
            <>
              <ArticlePatchButtons
                blogId={article.blogId}
                postId={article.id}
                userData={article.userData}
              />
              {renderVisitPageButton(article.url)}
              <Button onClick={() => push(`/blog/${blogid}`)}>Return</Button>
            </>
          }
          title={<Typography variant="h4">{article.name}</Typography>}
          subheader={`${article.publishDate}, ${article.author}`}
        />
        <Divider />
        <SingleArticleWrapper>{parse(article.content)}</SingleArticleWrapper>
      </>
    )
  }

  return <>{renderArticle()}</>
}

export default SingleArticle
