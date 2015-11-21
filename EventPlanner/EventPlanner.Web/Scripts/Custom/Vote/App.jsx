var Options = {
    YES : 'Yes',
    MAYBE : 'Maybe',
    NO : 'No'
};

var MappingHelper = {
    mapOption: function(option){
        return {
            title : option.Title,
            desc: option.Desc,
            optionId: option.Id,
            preSelectedValue: option.UsersVote.WillAttend,
            usersVoteId: option.UsersVote.Id,
            votes: {
                yes: option.Votes.Yes,
                maybe: option.Votes.Maybe,
                no: option.Votes.No
            }
        }
    }
};

var Tooltip = React.createClass({
    propTypes: {
        data: React.PropTypes.string,
    },
    render: function() {
        return (
            <div />
        );
    }

});

var DatesVotingApp = React.createClass({
    propTypes: {
        eventId: React.PropTypes.string,
        submitVotesUrl: React.PropTypes.string,
        getInitialDataUrl: React.PropTypes.string
    },
    getInitialState: function() {
        return {
            options: [],
            totalNumberOfVoters: 0 
        };
    },
    getDefaultProps: function() {
        return {
            eventId: $('#DatesVotingApp').attr('data-event-id'),
            submitVotesUrl: $('#DatesVotingApp').attr('data-submit-vote-for-date-url'),
            getInitialDataUrl: $('#DatesVotingApp').attr('data-get-vote-for-date-model-url')      
        };
    },
    
    componentWillMount: function() {
        $.get(this.props.getInitialDataUrl).success((data)=>{
            console.log(data);
            var totalNumberOfVoters = data.TotalNumberOfVoters;
            var options = data.Options.map(MappingHelper.mapOption);
            this.setState({options: options, totalNumberOfVoters: totalNumberOfVoters});
        });
    },
    shouldComponentUpdate: function(nextProps, nextState) {
        console.log('shouldComponentUpdate called with nextState:');
        console.log(nextState);
        return true;
    },
    submitVote: function(timeSlotId, voteForDateId, willAttend){
        console.log('Submitting vote...');
        var data = {
            eventId: this.props.eventId,
            timeSlotId: timeSlotId,
            voteForDateId: voteForDateId,
            willAttend: willAttend
        };
        
        var self = this;
        var successCallback = function(data){
            console.log('Vote for data submitted successfully');
            var option = MappingHelper.mapOption(data.Option);
            
            var newOptions = self.state.options;
            var index = newOptions.findIndex((elem, index) => elem.optionId === timeSlotId);
            if(index > -1){
                newOptions[index].votes = option.votes;
                newOptions[index].usersVoteId = option.usersVoteId
            }

            self.setState({
                totalNumberOfVoters: data.totalNumberOfVoters,
                options: newOptions
            })
        };

        $.ajax({
            type: "POST",
            url: self.props.submitVotesUrl,
            data: data,
            success: successCallback,
            dataType: 'JSON'
        });
    },
    render: function() {
        return (
             <div className="row">
                <h3>Vote for dates:</h3>
                <div className="col-sm-12">
                    {
                        this.state.options.map((option) => {
                            return(

                                <VoteOption key={option.optionId}
                                     optionId={option.optionId}
                                     title={option.title}
                                     desc={option.desc}
                                     onVoteCallback={this.submitVote}
                                     preSelectedValue={option.preSelectedValue}
                                     usersVoteId={option.usersVoteId}
                                     votes={option.votes}
                                     totalNumberOfVoters={this.state.totalNumberOfVoters} />
                            )
                        })
                    }
                </div>
            </div>
        );
    }

});

var VoteOption = React.createClass({
    propTypes: {
        optionId: React.PropTypes.string.isRequired,
        usersVoteId: React.PropTypes.string,
        title: React.PropTypes.string.isRequired,
        desc: React.PropTypes.string.isRequired,
        onVoteCallback: React.PropTypes.func.isRequired,
        preSelectedValue: React.PropTypes.string,
        votes: React.PropTypes.object.isRequired
    },
    getVotersFor: function(voteType) {
        switch (voteType) {
          case Options.YES: return this.props.votes.yes;
          case Options.MAYBE: return this.props.votes.maybe;
          case Options.NO: return this.props.votes.no;
        }
    },
    componentWillReceiveProps: function(nextProps) {
        console.log('Vote option will recieve new props');
        console.log(nextProps);
    },
    render: function() {
        return (
            <div className="row">
                <div className="col-md-2">
                    <div className="text-primary">{this.props.title}</div>
                    <div className="text-muted">{this.props.desc}</div>
                </div>
                <div className="col-md-2">
                    <VoteOptionForm 
                        optionId={this.props.optionId} 
                        usersVoteId={this.props.usersVoteId}
                        onValueSelectedCallback={this.props.onVoteCallback} 
                        preSelectedValue={this.props.preSelectedValue} />
                </div>
                <div className="col-md-8">
                    <Graph 
                        yesVoters={this.getVotersFor(Options.YES)} 
                        maybeVoters={this.getVotersFor(Options.MAYBE)} 
                        noVoters={this.getVotersFor(Options.NO)} 
                        totalNumberOfVoters={this.props.totalNumberOfVoters} />
                </div>
            </div>
        );
    }

});

var VoteOptionForm = React.createClass({
    propTypes: {
        optionId: React.PropTypes.string.isRequired,
        usersVoteId: React.PropTypes.string,
        preSelectedValue: React.PropTypes.string,
        onValueSelectedCallback: React.PropTypes.func.isRequired
    },
    onValueSelected: function(value){
        console.log('%s selected for option with %s id of VOTE %s.', value, this.props.optionId, this.props.usersVoteId);
        this.props.onValueSelectedCallback(this.props.optionId, this.props.usersVoteId, value);
    },
    render: function() {
        return (
            <div>
                <label>
                    <input type="radio" name={this.props.optionId} value={Options.YES} onChange={()=>this.onValueSelected(Options.YES)} defaultChecked={this.props.preSelectedValue == Options.YES}/>
                    <i className="glyphicon glyphicon-ok yes-option" title="Yes"><span className="sr-only">{Options.YES}</span></i>
                </label>
                <label>
                    <input type="radio" name={this.props.optionId} value={Options.MAYBE} onChange={()=>this.onValueSelected(Options.MAYBE)} defaultChecked={this.props.preSelectedValue == Options.MAYBE}/>
                    <i className="glyphicon glyphicon-minus maybe-option" title="Maybe"><span className="sr-only">{Options.MAYBE}</span></i>
                </label>
                <label>
                    <input type="radio" name={this.props.optionId} value={Options.NO} onChange={()=>this.onValueSelected(Options.NO)} defaultChecked={this.props.preSelectedValue == Options.NO}/>
                    <i className="glyphicon glyphicon-remove no-option" title="No"><span className="sr-only">{Options.NO}</span></i>
                </label>
            </div>
        );
    }

});

var ProgressBar = React.createClass({
    propTypes: {
        percentage: React.PropTypes.number.isRequired,
        voters: React.PropTypes.array.isRequired,
        voteType: React.PropTypes.string
    },
    getProgressBarClasses : function() {
      return {
          'progress-bar': true,
          'progress-bar-success': this.props.voteType === Options.YES,
          'progress-bar-warning': this.props.voteType === Options.MAYBE,
          'progress-bar-danger': this.props.voteType === Options.NO,
        }
    },
    getInlineStyles: function() {
        return {width: this.props.percentage + '%'};
    },
    render: function() {
        // TODO add proper tooltion to display names of voters 
        return (
            <div className={classNames(this.getProgressBarClasses())} style={this.getInlineStyles()} title={this.props.voters.reduce(((prev, curr) => prev + ', ' + curr),"")}>
                <span className="" >{this.props.voters.length}</span>
            </div>
        );
    }

});

var Graph = React.createClass({
    propTypes: {
        yesVoters: React.PropTypes.array.isRequired,
        maybeVoters: React.PropTypes.array.isRequired,
        noVoters: React.PropTypes.array.isRequired,
        totalNumberOfVoters: React.PropTypes.number
    },
    getTotalNumberOfVotes: function() {
        return this.props.totalNumberOfVoters || this.props.yesVoters.length + this.props.maybeVoters.length + this.props.noVoters.length;
    },
    isGraphEmpty : function() {
        return (this.props.yesVoters.length + this.props.maybeVoters.length + this.props.noVoters.length) === 0;
    },
    getBarPercentageFor: function(voteType){
        var x = 0;
        switch (voteType) {
          case Options.YES: x = this.props.yesVoters.length; break;
          case Options.MAYBE: x = this.props.maybeVoters.length; break;
          case Options.NO: x = this.props.noVoters.length; break;
        }

        return Math.round((x*100) / this.getTotalNumberOfVotes(), 0);
    },
    componentWillReceiveProps: function(nextProps) {
        console.log('Vote Graph will recieve new props');
        console.log(nextProps);
    },
    render: function() {
        if (!this.isGraphEmpty())
        {
            return (
                <div className="progress">
                    <ProgressBar voteType={Options.YES} voters={this.props.yesVoters} percentage={this.getBarPercentageFor(Options.YES)} />
                    <ProgressBar voteType={Options.MAYBE} voters={this.props.maybeVoters} percentage={this.getBarPercentageFor(Options.MAYBE)} />
                    <ProgressBar voteType={Options.NO} voters={this.props.noVoters} percentage={this.getBarPercentageFor(Options.NO)} />
                </div>
            );
        }
        else
        {
            return(
                <div className="progress">
                    <div className="text-center text-success">
                        Be the first one to vote for this option! Total number of voters is {this.props.totalNumberOfVoters}.
                    </div>
                </div>
            );
        }
    }
});