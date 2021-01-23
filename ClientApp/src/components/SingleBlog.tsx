import { Post } from 'api/api.types'
import React, { useState } from 'react'
import { Button } from 'react-bootstrap'
import { useSelector } from 'react-redux'
import { useHistory, useParams } from 'react-router-dom'
import { subscriptionsSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import * as blogApi from '../api/blogApi'
import BlogCard from './AppHome/BlogCard'

export interface SingleBlogProps {}

type CustomFeed = { foo: string }
type CustomItem = { bar: number }

export const SingleBlog: React.FC<SingleBlogProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { id } = useParams<{ id: string }>()
  const subscriptionsList = useSelector(subscriptionsSlice.selectAll)
  const [posts, setPosts] = useState<Post[]>(null)
  const [executed, setExecuted] = useState(false)

  const fetchPostList = async (id: string) => {
    if (executed) return
    setExecuted(true)

    const numberId = parseInt(id)
    if (numberId == NaN) return

    const sub = subscriptionsList.find(el => el.id == numberId)
    if (sub == null) return

    const response = await blogApi.getPostsList(sub.blog.id)

    setPosts(response)
  }

  const renderPosts = () => {
    if (posts == null) return

    return posts.map(el => (
      <BlogCard
        key={el.title}
        id={0}
        title={el.title}
        description={el.summary}
      />
    ))
  }

  fetchPostList(id)

  return (
    <div style={{ marginTop: 15 }} className="container">
      <Button onClick={() => push('/')} variant="primary">
        Return {id}
      </Button>
      {renderPosts()}
    </div>
  )
}

export default SingleBlog
