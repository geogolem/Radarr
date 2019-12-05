import _ from 'lodash';
import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { createSelector } from 'reselect';
import createMovieSelector from 'Store/Selectors/createMovieSelector';
import createQueueItemSelector from 'Store/Selectors/createQueueItemSelector';
import JustwatchLinks from './JustwatchLinks';

function createMapStateToProps() {
  return createSelector(
    createMovieSelector(),
    createQueueItemSelector(),
    (movie, queueItem) => {
      const result = _.pick(movie, [
        'inCinemas',
        'monitored',
        'grabbed'
      ]);

      result.queueItem = queueItem;
      result.movieFile = movie.movieFile;

      return result;
    }
  );
}

const mapDispatchToProps = {
};

class JustwatchLinksConnector extends Component {

  //
  // Render

  render() {
    return (
      <JustwatchLinks
        {...this.props}
      />
    );
  }
}

JustwatchLinksConnector.propTypes = {
  movieId: PropTypes.number.isRequired
};

export default connect(createMapStateToProps, mapDispatchToProps)(JustwatchLinksConnector);