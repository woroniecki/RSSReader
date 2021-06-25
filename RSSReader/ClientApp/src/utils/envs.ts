export const API_URL = () => {
  if (!process.env.NODE_ENV || process.env.NODE_ENV === 'development') {
    return process.env.REACT_APP_DEVELPOMENT_API_URL // will return API URL in .env file.
  } else {
    return process.env.REACT_APP_PRODUCTION_API_URL
  }
}
