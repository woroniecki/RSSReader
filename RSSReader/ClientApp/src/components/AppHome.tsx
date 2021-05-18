import React from 'react'
import { useHistory, useParams } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { AddSub } from './Blog/AddSub'
import { BlogCard } from './Blog/BlogCard'
import { getSubscribtionsList } from '../api/blogApi'
import { authSlice, subscriptionsSlice } from 'store/slices'
import { subscriptionsAdapter } from 'store/slices/subscriptionsSlice'
import { useSelector } from 'react-redux'

export interface AppHomeProps {}

export const AppHome: React.FC<AppHomeProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { token } = useSelector(authSlice.stateSelector)
  const subscriptionsList = useSelector(subscriptionsSlice.selectAll)
  const { groupId } = useParams<{ groupId: string }>()

  const renderBlogList = () => {
    const noGroupId = parseInt(groupId)

    return subscriptionsList
      .filter(x => Number.isNaN(noGroupId) || x.groupId == noGroupId)
      .map(el => (
        <BlogCard
          key={el.id}
          id={el.id}
          title={el.blog.name}
          description={el.blog.description}
          imageUrl={el.blog.imageUrl}
        />
      ))
  }

  return (
    <div style={{ marginTop: 15 }} className="container">
      <AddSub />
      {renderBlogList()}
    </div>
  )
}

export default AppHome
