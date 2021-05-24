import env from 'react-dotenv'
import axios from 'api/axios'
import {
  Subscription,
  AddSubscriptionRequest,
  Post,
  PatchPostRequest,
  Group,
  AddGroupRequest,
  PatchSubscriptionRequest,
  RemoveGroupRequest,
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
    const res = await axios.get(
      `/api/blog/` + blogId.toString() + `/post/list/0`
    )
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

export const getGroupsList = async () => {
  try {
    const res = await axios.get(`/api/group/list`)
    const group_none: Group = {
      id: -1,
      name: 'None',
    }
    return [group_none].concat(res.data.result)
  } catch (error) {
    throw error.response
  }
}

export const postAddGroup = async (data: AddGroupRequest) => {
  try {
    const res = await axios.post(`/api/group/add`, data)
    return res.data.result as Group
  } catch (error) {
    throw error.response
  }
}

export const removeGroup = async (data: RemoveGroupRequest) => {
  try {
    const res = await axios.delete(`/api/group/remove`, {
      data: data,
    })
    return res.data.result as Group
  } catch (error) {
    throw error.response
  }
}

export const patchSubscriptionGroup = async (
  subId: number,
  newGroupId: number
) => {
  try {
    const res = await axios.patch(
      `/api/subscription/` +
        subId.toString() +
        `/set_group/` +
        newGroupId.toString()
    )
    return res.data.result as Subscription
  } catch (error) {
    throw error.response
  }
}

export const patchSubscription = async (data: PatchSubscriptionRequest) => {
  try {
    const res = await axios.patch(
      `/api/subscription/` + data.subId.toString() + `/update`,
      data
    )
    return res.data.result as Subscription
  } catch (error) {
    throw error.response
  }
}
