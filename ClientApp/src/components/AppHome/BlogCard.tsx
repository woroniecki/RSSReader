import React from 'react'
import { Card, Button } from 'react-bootstrap'
import Image from 'react-bootstrap/Image'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { subscriptionsSlice, articlesSlice } from 'store/slices'
import { useSelector } from 'react-redux'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faTrashAlt, faPlus } from '@fortawesome/free-solid-svg-icons'

export interface BlogCardProps {
  title: string
  description: string
  imageUrl: string
  id: number
}

export const BlogCard: React.FC<BlogCardProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const subscriptionsList = useSelector(subscriptionsSlice.selectAll)
  const articlesList = useSelector(articlesSlice.selectAll)

  const unsubcribeBlog = async (id: number) => {
    const promise = await dispatch(subscriptionsSlice.putUnsubscribeBlog(id))

    if (subscriptionsSlice.putUnsubscribeBlog.fulfilled.match(promise)) {
    } else {
    }
  }

  const updateArticlesList = async () => {
    const list_already_taken = articlesList.find(el => el.blogId == props.id)
    if (list_already_taken != null) return

    const promise = await dispatch(articlesSlice.getArticles(props.id))

    if (articlesSlice.getArticles.fulfilled.match(promise)) {
    } else {
    }
  }

  function getUnreadedAmount() {
    updateArticlesList()

    const amount = articlesList.filter(
      el => el.blogId == props.id && !el.readed
    ).length

    if (amount <= 0) return

    return (
      <React.Fragment>
        <FontAwesomeIcon icon={faPlus} />
        <b>{amount}</b>
      </React.Fragment>
    )
  }

  const no_blog_img_url =
    'https://www.pngfind.com/pngs/m/269-2693798_png-file-svg-blog-vector-icon-png-transparent.png'

  return (
    <div className="container-fluid" style={{ marginTop: 5 }}>
      <div className="row">
        <div className="col-12 mt-3">
          <Card>
            <div className="card-horizontal">
              <div className="img-square-wrapper">
                <Image
                  width={100}
                  src={props.imageUrl ? props.imageUrl : no_blog_img_url}
                />
              </div>
              <Card.Body>
                <Card.Title>{props.title}</Card.Title>
                <Card.Text>{props.description}</Card.Text>
                <Button
                  onClick={() => push(`/blog/${props.id}`)}
                  variant="primary"
                >
                  Read
                </Button>
              </Card.Body>
              <div className="img-square-wrapper">
                {getUnreadedAmount()}
                <Button
                  onClick={() => {
                    unsubcribeBlog(props.id)
                  }}
                  variant="primary"
                >
                  <FontAwesomeIcon icon={faTrashAlt} />
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
