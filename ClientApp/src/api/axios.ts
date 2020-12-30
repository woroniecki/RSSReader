import axios, { AxiosRequestConfig } from 'axios'
import env from 'react-dotenv'

const axiosRequestConfig: AxiosRequestConfig = {
  baseURL: env.API_URL,
  headers: { Version: '1.0', 'Content-Type': 'application/json' },
}

const instance = axios.create(axiosRequestConfig)
export default instance
