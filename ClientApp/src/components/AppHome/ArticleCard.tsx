import React from 'react'
import { Card, Button } from 'react-bootstrap'
import Image from 'react-bootstrap/Image'
import { useHistory, useParams } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faStar } from '@fortawesome/free-solid-svg-icons'
import { faBook } from '@fortawesome/free-solid-svg-icons'

export interface ArticleCardProps {
  id: number
  title: string
  summary: string
  content: string
  url: string
  imageUrl: string
}

export const ArticleCard: React.FC<ArticleCardProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { id } = useParams<{ id: string }>()

  return (
    <div className="container-fluid" style={{ marginTop: 5 }}>
      <div className="row">
        <div className="col-12 mt-3">
          <Card>
            <div className="card-horizontal">
              <Card.Body>
                <Card.Title>{props.title}</Card.Title>
                <Card.Text>{props.summary}</Card.Text>
                <Button
                  onClick={() => push(`/blog/${id}/article/${props.id}`)}
                  variant="primary"
                >
                  Read
                </Button>
                <Button>
                  <FontAwesomeIcon icon={faStar} />
                </Button>
                <Button>
                  <FontAwesomeIcon icon={faBook} />
                </Button>
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
