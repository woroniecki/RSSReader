import React from 'react'
import { useHistory, useParams } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { AddSub } from './Blog/AddSub'
import { BlogCard } from './Blog/BlogCard'
import { getSubscribtionsList } from '../api/blogApi'
import { authSlice, subscriptionsSlice, layoutSlice } from 'store/slices'
import { subscriptionsAdapter } from 'store/slices/subscriptionsSlice'
import { useSelector } from 'react-redux'

export interface AppHomeProps {}

export const AppHome: React.FC<AppHomeProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { token } = useSelector(authSlice.stateSelector)
  const subscriptionsList = useSelector(subscriptionsSlice.selectAll)
  const { groupId } = useParams<{ groupId: string }>()
  const { loader } = useSelector(layoutSlice.stateSelector)

  const renderBlogList = () => {
    const noGroupId = parseInt(groupId)

    return subscriptionsList
      .filter(
        x =>
          Number.isNaN(noGroupId) ||
          x.groupId == noGroupId ||
          (noGroupId == -1 && x.groupId == null)
      )
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

  const renderMainPage = () => {
    if (token) {
      return (
        <div style={{ marginTop: 15 }} className="container">
          <AddSub activeGroupId={groupId} />
          {renderBlogList()}
        </div>
      )
    } else if (loader == layoutSlice.type.none) {
      return (
        <div style={{ marginTop: 15 }} className="container">
          MAIN PAGE
        </div>
      )
    }
  }

  return (
    <div style={{ marginTop: 15 }} className="container">
      {renderMainPage()}
    </div>
  )
}

export default AppHome
