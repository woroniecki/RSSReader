import env from 'react-dotenv'
import axios from 'api/axios'
import {
  Subscription,
  AddSubscriptionRequest,
  Post,
  PatchPostRequest,
} from './api.types'

export const getSubscribtionsList = async () => {
  try {
    const res = await axios.get(`/api/subscription/list`)
    return res.data.result as Subscription[]
  } catch (error) {
    throw error.response
  }
}

export const postAddSubscribtions = async (data: AddSubscriptionRequest) => {
  try {
    const res = await axios.post(`/api/subscription/subscribe`, data)
    return res.data.result as Subscription
  } catch (error) {
    throw error.response
  }
}

export const putUnsubscribeBlog = async (id: number) => {
  try {
    const res = await axios.put(`/api/subscription/${id}/unsubscribe`)
    return res.data.result as Subscription
  } catch (error) {
    throw error.response
  }
}

export const getPostsList = async (blogId: number) => {
  try {
    const res = await axios.get(`/api/blog/` + blogId.toString() + `/posts/0`)
    return res.data.result as Post[]
  } catch (error) {
    throw error.response
  }
}

export const putReadPost = async (blogId: number, postId: number) => {
  try {
    const res = await axios.put(
      `/api/blog/` + blogId.toString() + `/post/` + postId.toString()
    )
    return res.data.result as Post
  } catch (error) {
    throw error.response
  }
}

export const patchPost = async (data: PatchPostRequest) => {
  try {
    const res = await axios.patch(
      `/api/blog/` +
        data.blogId.toString() +
        `/post/` +
        data.postId.toString() +
        `/update`,
      data
    )
    return res.data.result as Post
  } catch (error) {
    throw error.response
  }
}
