import React from 'react';

const HttpRequest = ({ path, body, requestType, variables }) => {
  const requestTypes = {
    GET: 'GET',
    PUT: 'PUT',
    POST: 'POST',
    DELETE: 'DELETE',
  };

  const sendHttpRequest = async () => {
    let url = path;
    if (Object.keys(variables).length > 0) {
      Object.entries(variables).forEach(([key, value]) => {
        url = url.replace(`:${key}`, value);
      });
    }

    const requestOptions = {
      method: requestType,
      headers: { 'Content-Type': 'application/json' },
    };

    if (requestType !== requestTypes.GET && body) {
      requestOptions.body = JSON.stringify(body);
    }

    try {
      const response = await fetch(url, requestOptions);
      const responseData = await response.json();

      onResponse(response.status, responseData);
    } catch (error) {
      console.error(error);
      onResponse(null, error.message || 'HTTP request failed');
    }
  };

  return (
    <div>
      <h2>HTTP Request Example</h2>
      <button onClick={sendHttpRequest}>Send Request</button>
    </div>
  );
};

export default HttpRequest;