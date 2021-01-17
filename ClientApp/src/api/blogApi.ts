import env from 'react-dotenv'
import axios from 'api/axios'
import { Subscription, AddSubscriptionRequest } from './api.types'

export const getSubscribtionsList = async () => {
  try {
    const res = await axios.get(`/api/subscription/list`)
    return res.data.result as Subscription[]
  } catch (error) {
    throw error
  }
}

export const postAddSubscribtions = async (data: AddSubscriptionRequest) => {
  try {
    const res = await axios.post(`/api/subscription/subscribe`, data)
    return res.data.result as Subscription
  } catch (error) {
    throw error
  }
}
