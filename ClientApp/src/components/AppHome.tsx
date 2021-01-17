import React from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { AddSub } from './AppHome/AddSub'
import { BlogCard } from './AppHome/BlogCard'
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

  const fetchList = async () => {
    const promise = await dispatch(subscriptionsSlice.getList())

    if (subscriptionsSlice.getList.fulfilled.match(promise)) {
    } else {
    }
  }
  React.useEffect(() => {
    if (token) {
      fetchList()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token])

  const renderBlogList = () =>
    subscriptionsList.map(el => (
      <BlogCard
        key={el.id}
        id={el.id}
        title={el.blog.name}
        description={el.blog.name}
      />
    ))

  return (
    <div style={{ marginTop: 15 }} className="container">
      <AddSub />
      {renderBlogList()}
    </div>
  )
}

export default AppHome
