import React from 'react'
import { Card, Button } from 'react-bootstrap'
import Image from 'react-bootstrap/Image'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { subscriptionsSlice } from 'store/slices'
import { useSelector } from 'react-redux'

export interface BlogCardProps {
  title: string
  description: string
  id: number
}

export const BlogCard: React.FC<BlogCardProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const subscriptionsList = useSelector(subscriptionsSlice.selectAll)

  const unsubcribeBlog = async (id: number) => {
    const promise = await dispatch(subscriptionsSlice.putUnsubscribeBlog(id))

    if (subscriptionsSlice.putUnsubscribeBlog.fulfilled.match(promise)) {
    } else {
    }
  }

  return (
    <div className="container-fluid" style={{ marginTop: 5 }}>
      <div className="row">
        <div className="col-12 mt-3">
          <Card>
            <div className="card-horizontal">
              <div className="img-square-wrapper">
                <Image src="https://picsum.photos/150/150" />
              </div>
              <Card.Body>
                <Card.Title>{props.title}</Card.Title>
                <Card.Text>{props.description}</Card.Text>
                <Button
                  onClick={() => push(`/blog/${props.id}`)}
                  variant="primary"
                >
                  Open
                </Button>
              </Card.Body>
              <div className="img-square-wrapper">
                <Button
                  onClick={() => {
                    unsubcribeBlog(props.id)
                  }}
                  variant="primary"
                >
                  Unsubscribe
                </Button>
              </div>
            </div>
          </Card>
        </div>
      </div>
    </div>
  )
}

export default BlogCard
