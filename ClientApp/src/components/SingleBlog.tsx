import React from 'react'
import { Button } from 'react-bootstrap'
import { useSelector } from 'react-redux'
import { useHistory, useParams } from 'react-router-dom'
import { subscriptionsSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'

export interface SingleBlogProps {}

export const SingleBlog: React.FC<SingleBlogProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { id } = useParams<{ id: string }>()
  const subscriptionsList = useSelector(subscriptionsSlice.selectAll)

  const renderBlog = (id: string) => {
    const numberId = parseInt(id)
    if (numberId == NaN) return 'Something wrong'

    const sub = subscriptionsList.find(el => el.id == numberId)
    if (sub == null) return 'Something wrong'
    return <div>{sub.blog.name}</div>
  }

  return (
    <div style={{ marginTop: 5 }}>
      <Button onClick={() => push('/')} variant="primary">
        Return {id}
        {renderBlog(id)}
      </Button>
    </div>
  )
}

export default SingleBlog
