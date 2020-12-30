import axios from 'api/axios'
import env from 'react-dotenv'
import { Subscription } from './api.types'

export const getSubscribtionsList = async () => {
  try {
    const res = await axios.get(`/api/subscription/list`)
    return res.data as Subscription[]
  } catch (error) {
    throw error
  }
}
