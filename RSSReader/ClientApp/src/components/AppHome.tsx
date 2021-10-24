import React from 'react'
import { useParams } from 'react-router-dom'
import { AddSub } from './Blog/AddSub'
import { BlogCard } from './Blog/BlogCard'
import { authSlice, blogsSlice, layoutSlice } from 'store/slices'
import { useSelector } from 'react-redux'
import { Typography } from '@material-ui/core'

export interface AppHomeProps {}

export const AppHome: React.FC<AppHomeProps> = () => {
  const { token } = useSelector(authSlice.stateSelector)
  const blogsList = useSelector(blogsSlice.selectAll)
  const { groupId } = useParams<{ groupId: string }>()
  const { loader } = useSelector(layoutSlice.stateSelector)

  const renderBlogList = () => {
    const noGroupId = parseInt(groupId)

    return blogsList
      .filter(
        el =>
          Number.isNaN(noGroupId) ||
          (el.userData != null && el.userData.groupId == noGroupId) ||
          (el.userData != null &&
            noGroupId == -1 &&
            el.userData.groupId == null)
      )
      .map(el => <BlogCard blog={el} />)
  }

  const renderMainPage = () => {
    if (token) {
      return (
        <>
          <AddSub activeGroupId={groupId} />
          {renderBlogList()}
        </>
      )
    } else if (loader == layoutSlice.type.none) {
      return <Typography>MAIN PAGE</Typography>
    }
  }

  return <div>{renderMainPage()}</div>
}

export default AppHome
