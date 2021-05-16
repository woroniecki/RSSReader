import React from 'react'
import { Card, Button } from 'react-bootstrap'
import Image from 'react-bootstrap/Image'
import { useHistory, useParams } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faStar } from '@fortawesome/free-solid-svg-icons'
import { faBook } from '@fortawesome/free-solid-svg-icons'
import { faBookOpen } from '@fortawesome/free-solid-svg-icons'
import { articlesSlice } from 'store/slices'
import ArticlePatchButtons from './ArticlePatchButtons'

export interface ArticleCardProps {
  id: number
  title: string
  summary: string
  content: string
  url: string
  imageUrl: string
  readed: boolean
  favourite: boolean
  publishDate: string
}

export const ArticleCard: React.FC<ArticleCardProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { id } = useParams<{ id: string }>()

  const blogIdNumber = parseInt(id)

  function getClassNames(transparent: boolean): string {
    return 'card-horizontal' + (transparent ? ' transparent' : '')
  }

  return (
    <div className="container-fluid" style={{ marginTop: 5 }}>
      <div className="row">
        <div className="col-12 mt-3">
          <Card>
            <div className={getClassNames(props.readed)}>
              <Card.Body>
                <Card.Title>{props.title}</Card.Title>
                <Card.Text>{props.publishDate}</Card.Text>
                <Card.Text>{props.summary}</Card.Text>
                <Button
                  onClick={() => push(`/blog/${id}/article/${props.id}`)}
                  variant="primary"
                >
                  Read
                </Button>
                <ArticlePatchButtons
                  blogId={blogIdNumber}
                  postId={props.id}
                  readed={props.readed}
                  favourite={props.favourite}
                />
              </Card.Body>
              <div className="img-square-wrapper">
                <Image src={props.imageUrl} className="const-img-150" />
              </div>
            </div>
          </Card>
        </div>
      </div>
    </div>
  )
}

export default ArticleCard