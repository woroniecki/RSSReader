import React from 'react'
import { useHistory } from 'react-router-dom'

export interface ListBlogProps {}

export const ListBlog: React.FC<ListBlogProps> = props => {
  const { push } = useHistory()

  return null
}

export default ListBlog
