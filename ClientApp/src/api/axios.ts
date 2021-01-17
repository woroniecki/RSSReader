import env from 'react-dotenv'
import axios, { AxiosRequestConfig } from 'axios'

const axiosRequestConfig: AxiosRequestConfig = {
  baseURL: env.API_URL,
  // baseURL: 'env.API_URL',
  headers: { Version: '1.0', 'Content-Type': 'application/json' },
}

const instance = axios.create(axiosRequestConfig)
export default instance
